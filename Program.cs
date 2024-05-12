using app.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Net;
using System.Net.WebSockets;
using System.Net.Sockets;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectString")));

var _authkey = builder.Configuration.GetValue<string>("JwtSettings:securitykey");
//=======TAO COOKIE=========
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authkey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew=TimeSpan.Zero
    };
    //ki gui token vao cookie
    item.Events = new JwtBearerEvents{
        OnMessageReceived = context =>{
            var token = context.Request.Cookies["token"];
            context.Token = token;
            return Task.CompletedTask;
        }
    };
});
//=======TAO COOKIE=========
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IStudentService, StudentService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var _jwtSetting = builder.Configuration.GetSection("JwtSettings");

builder.Services.Configure<JwtSettings>(_jwtSetting);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
        response.StatusCode == (int)HttpStatusCode.Forbidden)
        response.Redirect("/login");
});

app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();
var connections = new List<WebSocket>();
app.Map("/ws", async context =>{
    if(context.WebSockets.IsWebSocketRequest){
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        connections.Add(ws);
        await BroadCast("joineddd okok");
        await ReceiMessage(ws, 
            async (result, buffer)=>{
                if(result.MessageType == WebSocketMessageType.Text){
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await BroadCast(message);

                }else if(result.MessageType==WebSocketMessageType.Close || ws.State==WebSocketState.Aborted){
                    connections.Remove(ws);
                    await BroadCast("left room");
                    await BroadCast($@"{connections.Count} is connecting");
                    await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            }
        );
    }else{
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

    }
});
async Task BroadCast(string message){
    var bytes = Encoding.UTF8.GetBytes(message);
    foreach(var socket in connections){
        if(socket.State==WebSocketState.Open){
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length) ;
            await socket.SendAsync(arraySegment, 
            WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
 }
async Task ReceiMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage){
    var buffer = new byte[1024];
    while(socket.State == WebSocketState.Open){
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        handleMessage(result, buffer);
    }
}
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
