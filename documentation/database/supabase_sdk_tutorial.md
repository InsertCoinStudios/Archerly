Link to the Documentation:
https://supabase.com/docs/reference/csharp/introduction

# Starting Note
This MD File is subject to change as development progresses. Most importantly the section on User Creation and Authentification is sure to be further expanded to integrate the custom user data to supabase's basic user data.

### Endpoint Authentifikation

Found for service authentication api keys are found [here](https://supabase.com/dashboard/project/xvbnlycdrewhoyhulylj/settings/api-keys/legacy). 

Standard Initialization using the SDK:

```c#
var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");

var options = new Supabase.SupabaseOptions
{
    AutoConnectRealtime = true
};

var supabase = new Supabase.Client(url, key, options);
await supabase.InitializeAsync();
```

## User Auth

#### Create New User
```c#
var session = await supabase.Auth.SignUp(email, password);

//we need additional functionality for username and such though!!
```
#### Sign-In User
```c#
var session = await supabase.Auth.SignIn(email, password);
```
#### Get Cur User or Session
```c#
var user = supabase.Auth.CurrentUser;
var session = supabase.Auth.CurrentSession;
```
## Data Requests

Example animal table:
https://supabase.com/dashboard/project/xvbnlycdrewhoyhulylj/api?resource=animal
You can literally copy and paste the js or bash requests.
Like any other API-Doc, developers can take a look where to request data and what kind of data will be returned.


![[Pasted image 20251204160924.png]]

#### Fetch Data

Example Select with Inner Join and where condition.

```c#
var result = await supabase .From<Movie>() .Select("*, users!inner(*)") .Filter("user.username", Operator.Equals, "Jane") .Get();
```
https://supabase.com/docs/reference/csharp/select

#### Insert Data

```c#
[Table("cities")]
class City : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }
}

var model = new City
{
  Name = "The Shire",
  CountryId = 554
};

await supabase.From<City>().Insert(model);
```

#### Update Data

```c#
var update = await supabase 
.From<City>() 
.Where(x => x.Name == "Auckland") 
.Set(x => x.Name, "Middle Earth") 
.Update();
```

#### Delete Data
```c#
await supabase
  .From<City>()
  .Where(x => x.Id == 342)
  .Delete();
```