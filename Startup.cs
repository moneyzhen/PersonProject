using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;

namespace ZHONGJIAN_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigHelper._configuration = configuration;
        }
        
        public IConfiguration Configuration { get; }

        private readonly string AllowSpecificOrigin = "AllowSpecificOrigin";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            services.AddHttpClient();
            //�޷�����������ʱ��json�� ��Ҫ��Ϊ������ִ��Ч�ʣ�System.Text.Json��Ϊ΢������json����Ч�ʸ��߸��졣΢��ٷ���jsonֻ������ĸ�ʽ2020 - 08 - 18T08: 00�����պ�ʱ����֮���һ��T
            services.AddControllers().AddNewtonsoftJson();

            #region ����
            services.AddCors(options =>
            {
                options.AddPolicy(AllowSpecificOrigin,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
          
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            //��������������Դ���� CORS �м����������Ϊ�ڶ� UseRouting �� UseEndpoints�ĵ���֮��ִ�С���UseAuthorization֮�� ���ò���ȷ�������м��ֹͣ�������С�
            app.UseCors(AllowSpecificOrigin);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
