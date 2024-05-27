namespace FlightInformationApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
            var allowOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy(name: allowOrigins,
            //        policy =>
            //        {
            //            policy.WithOrigins("http://localhost:8888", "http://localhost:8889", "https://flightdata.andynet.se");
            //        });
            //});

            // Add services to the container.
            builder.Services.AddControllersWithViews();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				//app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

            //app.UseCors(allowOrigins);

            app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
