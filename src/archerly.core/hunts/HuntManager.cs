using Microsoft.AspNetCore.Http;
using Serilog;
using System.Linq;

namespace archerly.core.hunts;

public class HuntManager : IDisposable
{
    private readonly SessionManager _sessions;
    private const short _maxSessions = 4096;

    public HuntManager(bool cleanUp, long intervalInMinutes)
    {
        if (cleanUp)
        {
            _sessions = SessionManager.WithAutomaticCleanup(intervalInMinutes);
        }
        else
        {
            _sessions = new();
        }
    }

    public string CreateNewPendingHunt(Guid ownerId)
    {
        if (_sessions.Count > _maxSessions)
        {
            var ex = new InvalidOperationException(
                $"Cannot create a new pending hunt: maximum number of sessions ({_maxSessions}) reached."
            );
            Log.Warning(ex, $"Location: CreateNewPendingHunt Creating PendingHunt for User with Guid ({ownerId})");
            throw ex;
        }
        var transitionAction = _sessions.TransitionFromPending;
        var transferFunc = TransferStrategies.DissolveOnOwnerLeave;
        PendingHunt pending = new(ownerId, transitionAction, transferFunc);

        var dissolveFunc = () => { _sessions.Remove(pending.SessionId); };
        pending.Players.RequestDissolution = dissolveFunc;
        _sessions.AddPendingHunt(pending);
        return pending.SessionId;
    }

    public void SetCourseForPendingHunt(string sessionId, Guid courseId)
    {
        // TODO: I am expecting potential exceptions from the database layer when retrieving the Course
        try
        {
            _sessions.SetCourse(sessionId, courseId);
        }
        catch (Exception e)
        {
            Log.Warning(e, "Function: SetCourseForPendingHunt");
            throw;
        }
    }

    public void SetScoringVariantForPendingHunt(string sessionId, int scoringVariant)
    {
        try
        {
            _sessions.SetScoringVariant(sessionId, scoringVariant);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Log.Warning(
                        ex,
                        "Failed to set scoring variant for pending hunt. SessionId: {SessionId}, Value: {ScoringVariant}",
                        sessionId,
                        scoringVariant
                    );
            throw;
        }
    }

    public bool IsOwnerOf(string sessionId, Guid user)
    {
        var session = _sessions.GetSession(sessionId);
        if (session.IsHunt())
        {
            return session.Hunt.Players.Owner.Equals(user);
        }
        if (session.IsPending())
        {
            return session.Pending.Players.Owner.Equals(user);
        }
        return false;
    }

    public void RemoveUserFromSessions(Guid userId)
    {
        _sessions.RemovePlayerFromSessions(userId);
    }

    public void ActivatePendingHunt(string sessionId)
    {
        try
        {
            _sessions.ActivateSession(sessionId);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: ActivatePendingHunt ID: {sessionId}");
        }
    }

    public void RemoveSession(string sessionId)
    {
        _sessions.Remove(sessionId);
    }
    public bool PlayerJoined(string sessionId, Guid playerId)
    {
        return _sessions.PlayerJoined(sessionId, playerId);
    }

    public bool PlayerLeft(string sessionId, Guid playerId)
    {
        return _sessions.PlayerLeft(sessionId, playerId);
    }

    // TODO: Accept Shot Made Call to the Hunt
    public void SaveShot(string sessionId, Guid playerId, Guid animalId, long points)
    {
        try
        {
            _sessions.RegisterShot(sessionId, playerId, animalId, points);
        }
        catch (Exception e)
        {
            Log.Warning(e, $"Function: Saveshot ID: {sessionId} Guid: {playerId} ANimalId:{animalId}");
            throw;
        }
    }

    public AllStats GetStatsFor(string sessionId)
    {
        try
        {
            var stats = _sessions.GetStats(sessionId);
            return stats;
        }
        catch (Exception e)
        {
            Log.Warning(e, $"{nameof(GetStatsFor)} ID: {sessionId}");
            throw;
        }
    }

    public UserStats GetUserStatsFor(string sessionId, Guid user)
    {
        try
        {
            var stats = _sessions.GetUserStats(sessionId, user);
            return stats;
        }
        catch (Exception e)
        {
            Log.Warning(e, $"{nameof(GetStatsFor)} ID: {sessionId}");
            throw;
        }
    }

    public SessionData? GetDataFor(string sessionId, Guid requester)
    {
        IReadOnlyList<Guid> players;
        Guid owner = Guid.Empty;
        string sessionCode;
        var session = _sessions.GetSession(sessionId);

        if (session.IsHunt())
        {
            var list = session.Hunt.Players;
            players = list.ToList;
            if (!players.Contains(requester))
            {
                Log.Information($"Requester {requester} tried retrieving Data of Session {sessionId} without being a member of the Session");
                return null;
            }
            owner = list.Owner;
            sessionCode = session.Hunt.SessionId;
            return new SessionData(players, owner, sessionCode);
        }
        if (session.IsPending())
        {
            var list = session.Pending.Players;
            players = list.ToList;
            if (!players.Contains(requester))
            {
                Log.Information($"Requester {requester} tried retrieving Data of Session {sessionId} without being a member of the Session");
                return null;
            }
            owner = list.Owner;
            sessionCode = session.Pending.SessionId;
            return new SessionData(players, owner, sessionCode);
        }
        return null;
    }

    public record SessionData(IReadOnlyList<Guid> Players, Guid Owner, string SessionId);

    public void Dispose()
    {
        _sessions.Dispose();
    }

    private class TransferStrategies
    {

        public static Guid DissolveOnOwnerLeave(PlayerList players)
        {
            players.RequestDissolution();
            return Guid.Empty;
        }

        public static Guid TransferToTop(PlayerList players)
        {
            return players.ToList.FirstOrDefault();
        }
    }
}