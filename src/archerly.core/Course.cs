namespace archerly.core;

public class Course
{
    public string Name { get; }
    public string Location { get; }
    public string Info { get; }
    public Difficulty Difficulty { get; }
    public List<Animal> Targets
    {
        get
        {
            lock (_targetsLock)
            {
                return _targets;
            }
        }
    }
    private readonly List<Animal> _targets;
    private readonly Lock _targetsLock = new();

    internal Course(PartialCourse partial, List<Animal> targets)
    {
        if (targets == null || targets.Count == 0)
        {
            throw new ArgumentException("Targets cannot be null or empty");
        }
        Name = partial.Name;
        Location = partial.Location;
        Info = partial.Info;
        Difficulty = partial.Difficulty;
        _targets = new(targets);
    }

    public Course(string name, string location, string info, Difficulty difficulty, List<Animal> targets)
    {
        Name = name;
        Location = location;
        Info = info;
        Difficulty = difficulty;
        _targets = targets;
    }

    // Placeholder for call to db to retrieve Course
    public Course(Guid id)
    {
        _ = id;
        Name = string.Empty;
        Location = string.Empty;
        Info = string.Empty;
        Difficulty = Difficulty.Easy;
        _targets = new List<Animal>();
    }

    public void AddTarget(Animal target)
    {
        ArgumentNullException.ThrowIfNull(target);
        lock (_targetsLock)
        {
            _targets.Add(target);
        }
    }

    public void RemoveTarget(Animal target)
    {
        ArgumentNullException.ThrowIfNull(target);
        lock (_targetsLock)
        {
            _targets.Remove(target);
        }
    }

    public void MoveTarget(int oldIndex, int newIndex)
    {
        lock (_targetsLock)
        {
            if (oldIndex < 0 || oldIndex >= _targets.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(oldIndex));
            }
            if (newIndex < 0 || newIndex >= _targets.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(newIndex));
            }

            var animal = _targets[oldIndex];
            _targets.RemoveAt(oldIndex);

            // Adjust newIndex if removal affects it
            if (newIndex > oldIndex)
            {
                newIndex--;
            }
            _targets.Insert(newIndex, animal);
        }
    }

    public List<Guid> GetTargetOrderIds()
    {
        lock (_targetsLock)
        {
            return _targets.Select(a => a.Id).ToList();
        }
    }

    public void ApplyTargetOrder(List<Guid> orderedIds)
    {
        ArgumentNullException.ThrowIfNull(orderedIds);

        lock (_targetsLock)
        {
            if (orderedIds.Count != _targets.Count ||
                !orderedIds.All(id => _targets.Any(a => a.Id == id)))
            {
                throw new ArgumentException("Invalid target order");
            }

            // Reorder targets in-place
            var dict = _targets.ToDictionary(a => a.Id);
            _targets.Clear();
            foreach (var id in orderedIds)
            {
                _targets.Add(dict[id]);
            }
        }
    }
}