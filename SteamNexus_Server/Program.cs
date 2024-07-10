using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SteamNexus_Server.Data;
using SteamNexus_Server.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// �[�J CORS ����
string MyAllowSpecificOrigins = "AllowAny";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy => policy.WithOrigins("https://www.steamnexus.org" , "http://localhost:5173").WithMethods("*").WithHeaders("*")
    );
});

#region cookie����

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
//{
//    //���n�J�ɥ��۰ʾɤJ��o�Ӻ��}
//    option.LoginPath = new PathString("/api/Login/NoLogin");

//    //�����v����|�۰ʲ���즹���}
//    option.AccessDeniedPath = new PathString("/api/Login/noAccess");

//    //�n�J����ᥢ�ġF������cookie���|����v�T
//    //option.ExpireTimeSpan = TimeSpan.FromSeconds(5);
//});

#endregion


#region JWT����
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        //�o�������
        ValidateIssuer = true,
        // �]�m���Ī��o���
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //����������
        ValidateAudience = true,
        //�]�m���Ī�������
        ValidAudience = builder.Configuration["Jwt:Audience"],
        //�n�J�ɶ����ҡA�w�]�Otrue�A�i�g�i���g
        ValidateLifetime = true,
        //���� Token ��ñ�W���_
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//API���n����n�J������
//builder.Services.AddMvc(options =>
//{
//    options.Filters.Add(new AuthorizeFilter());
//});

#endregion


// DataBase Connection String
var SteamNexusConnectionString = builder.Configuration.GetConnectionString("SteamNexus");
// Add SteamNexusDbContext
builder.Services.AddDbContext<SteamNexusDbContext>(options => options.UseSqlServer(SteamNexusConnectionString));


// Add CoolPCWebScrabing Service
builder.Services.AddTransient<CoolPCWebScraping>();
builder.Services.AddTransient<GameTimer>();

// ���U�p�ɾ��A��
builder.Services.AddTransient<ScheduledTaskService>();


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

// �ҥ� wwwroot
app.UseStaticFiles();

//cookie�n�J
app.UseCookiePolicy();
app.UseAuthentication();

// �M�Φ۩w�q CORS �]�w
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();


app.Run();