using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text.Json;

public partial class customer
{
    public int CustomerID { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }

    public string Password {get; set;} 


}
public partial class helloappdbContext : DbContext
    {
        public helloappdbContext()
        {
        }
 
        public virtual DbSet<customer> customer { get; set; }
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;user=root;password=root123;database=testc;", new MySqlServerVersion(new Version(8, 0, 27)));
            }
        }
}
class StatusResponse{
    public string Status{get;set;}
    public StatusResponse(string status){
        Status = status;
    }
}
// class Person{
//     public string email{set;get;}
//     public string password{set;get;}
//     // public Person(string Email,string Password){
//     //     email = Email;
//     //     password = Password;
//     // }
// }
public record Person(string Email, string Password);
class Program{
static void Main(){
    helloappdbContext db = new helloappdbContext();
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions { WebRootPath = "templates"});
    string myorigins = "myorigins";
    builder.Services.AddCors(options => {
        options.AddPolicy(
            name:myorigins,
            policy  =>
                {
                          policy.WithOrigins("http://localhost:3000");
                });

    });
    var app = builder.Build();
    app.UseCors(myorigins);
    app.UseStaticFiles();
    app.Map("/reg", async(context)=>{
        var response = context.Response;
        var request = context.Request;
        if (request.Method =="POST"){
            var forms = request.Form;
            customer NewUser = new customer{LastName=forms["lastname"],FirstName=forms["firstname"],Email=forms["email"],Password=forms["password"]};
            Console.WriteLine("POST");
            try{
                db.customer.Add(NewUser);
                db.SaveChanges();
                Console.WriteLine("Succesfully added");
                context.Response.Redirect("/");
            }
            catch (Exception e){
                Console.WriteLine(e);
            }
        }
        await response.SendFileAsync("templates/reg.html");
        // var form = context.Request.Form;
    });
    app.Map("/login", async(context)=>{
        var request = context.Request;
        var response = context.Response;
        StatusResponse feedback;
        string res;
        switch(request.Method){
            case "POST":
                var person = await request.ReadFromJsonAsync<Person>();
                Console.WriteLine($"{person.Email},{person.Password}");
                bool exist_user = await Task.Run(()=>db.customer.Any(user => user.Email == $"{person.Email}" && user.Password == $"{person.Password}"));
                switch(exist_user){
                    case true:
                        Console.WriteLine("found");
                        feedback = new StatusResponse("good");
                        // res = JsonSerializer.Serialize(feedback);
                        await response.WriteAsJsonAsync(feedback, new JsonSerializerOptions() { IncludeFields = true});
                        break;
                    case false:
                        Console.WriteLine("Not found");
                        feedback = new StatusResponse("not found");
                        res = JsonSerializer.Serialize(feedback);
                        await response.WriteAsJsonAsync(res);
                        break;
                }
                break;
        }
        await response.SendFileAsync("templates/login.html");
        
    });
    app.Run();
}
}

