using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.Services;
using SocialMedia.Infrastructure.Data;
using SocialMedia.Infrastructure.Extensions;
using SocialMedia.Infrastructure.Filters;
using SocialMedia.Infrastructure.Interfaces;
using SocialMedia.Infrastructure.Options;
using SocialMedia.Infrastructure.Repositories;
using SocialMedia.Infrastructure.Services;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace SocialMedia.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Agergando el Automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //GlobalExceptionFilter: Filtro para generar excepciones personalizadas
            //NewtonsoftJson..ReferenceLoopHandling: Serialización de Json para ignorar referencia circular
            //NewtonsoftJson..NullValueHandling: Serialización de Json para ignorar valores nulos, en nuestro caso el Metadata
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            }
            ).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            })

            //Quitar validación del modelo que controla el ApiController
            .ConfigureApiBehaviorOptions(options =>
            {
                //options.SuppressModelStateInvalidFilter = true;
            });

            //Agrega las opciones por defecto para la paginación desde el appsettings
            //services.Configure<PaginationOptions>(Configuration.GetSection("Pagination"));
            //Agrega las opciones por defecto para la encriptación del password desde el appsettings
            //services.Configure<PasswordOptions>(Configuration.GetSection("PasswordOptions"));
            services.AddOptions(Configuration);

            //Add DBContext
            //services.AddDbContext<SocialMediaContext>(options => 
            //    options.UseSqlServer(Configuration.GetConnectionString("SocialMedia"))
            //);
            services.AddDBContexts(Configuration);

            //Dependency Injection
            //services.AddTransient<IPostService, PostService>();
            //services.AddTransient<ISecurityService, SecurityService>();
            ////services.AddTransient<IPostRepository, PostRepository>();
            ////services.AddTransient<IUserRepository, UserRepository>();
            //services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            //services.AddTransient<IUnitOfWork, UnitOfWork>();
            //services.AddSingleton<IPasswordService, PasswordService>();
            //services.AddSingleton<IUriService>(provider =>
            //{
            //    var accesor = provider.GetRequiredService<IHttpContextAccessor>();
            //    var request = accesor.HttpContext.Request;
            //    var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
            //    return new UriService(absoluteUri);
            //});
            services.AddServices();

            //Agregar Swagger
            //services.AddSwaggerGen(doc =>
            //{
            //    doc.SwaggerDoc("v1", new OpenApiInfo { Title = "Social Media API", Version = "v1" });

            //    //Agregar los comentarios "summary" de cada método
            //    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    doc.IncludeXmlComments(xmlPath);
            //});
            //Se genera un json en la url localhost:port/swagger/v1/swagger.json
            services.AddSwagger($"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            //Agregar Authentication JWT, siempre agregarlo antes de MVC
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],
                    ValidAudience = Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]))
                };
            });

            //Agregar middleware de filtro, para validar los modelos de manera global
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            })
                //Registrar los validators
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            //Usar Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                //Para localhost o Azure
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Social Media API V1");
                options.RoutePrefix = string.Empty;

                //Para Local IIS
                //options.SwaggerEndpoint("../swagger/v1/swagger.json", "Social Media API V1");
            });
            //Habilita el SwaggerUI en locahot:port/swagger/index.html

            app.UseRouting();

            //Usa la autenticación configurada, antes de Authorization
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
