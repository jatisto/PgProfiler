using ElectronNET.API;
using ElectronNET.API.Entities;
using PgProfiler.Data;
using Blazored.Modal;
using MoiLib.Helpers;
using MudBlazor.Services;
using PgProfiler;
using PgProfiler.Interface;

var builder = WebApplication.CreateBuilder(args);

	builder.Services.AddRazorPages();
	builder.Services.AddServerSideBlazor();
	builder.Services.AddScoped<PgProfilerService>();
	builder.Services.AddScoped<ISettings, ContextSettings>();
	builder.Services.AddScoped<JsRuntimeForBlazorHelper>();
	builder.Services.AddScoped<IWorkingWithSettings, WorkingWithSettings>();
	builder.WebHost.UseElectron(args);
	builder.Services.AddBlazoredModal();
	builder.Services.AddMudBlazorScrollManager();
	builder.Services.AddMudServices();
	builder.Services.AddMudBlazorResizeObserver();
	builder.Services.AddMudBlazorResizeObserverFactory();
	builder.Services.AddMudBlazorDialog();

	var app = builder.Build();
	
	if (!app.Environment.IsDevelopment())
	{
		app.UseExceptionHandler("/Error");
		app.UseHsts();
	} 

	app.UseHttpsRedirection();

	app.UseStaticFiles();

	app.UseRouting();

	app.MapBlazorHub();
	app.MapFallbackToPage("/_Host");

	if (HybridSupport.IsElectronActive)
	{
		var window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions()
		{
			Title = "PgProfiler",
			Width = 1100,
			Height = 700,
			BackgroundColor = "#27272f",
			// Frame = false,
			Icon =
				Path.Combine("E:\\MoiRepositoryWork\\PgProfiler\\PgProfiler\\PgProfiler\\wwwroot\\assets\\icons\\icon.ico")
		});
		Electron.Tray.Show(
			Path.Combine("E:\\MoiRepositoryWork\\PgProfiler\\PgProfiler\\PgProfiler\\wwwroot\\assets\\icons\\icon.ico"));
		
		window.OnClosed += () =>
		{
			Electron.App.Exit(0);
			Environment.Exit(0);
			Electron.App.Quit();
			window = null;
		};
	}
	
	app.Run();