The steps to get this WinForms project working are:

1. Add a reference to your PCL project
2. Open Program.cs and replace the code to create IoC container:
	new Bootstrapper<Core.App>(new IIocContainer())
3. Remove any old forms