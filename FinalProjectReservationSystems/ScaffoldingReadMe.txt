Scaffolding has generated all the files and added the required dependencies.

However the Application's Startup code may require additional changes for things to work end to end.
Add the following code to the Configure method in your Application's Startup class if not already done:

        app.UseEndpoints(endpoints =>
        {
          endpoints.MapControllerRoute(
            name : "areas",
            pattern : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
        });









        "Default": "Data Source=SQL8002.site4now.net;Initial Catalog=db_a946ea_sona;User Id=db_a946ea_sona_admin;Password=huseyn2003"













        _toastNotification.AddErrorToastMessage(message: "Something went wrong.", new
                    ToastrOptions
                {
                    Title = "Opps!"
                });





                _toastNotification.AddSuccessToastMessage(message: "Process completed !", new
                    ToastrOptions
            {
                Title = "Successful"
            });