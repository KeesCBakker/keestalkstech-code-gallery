## 7. Docker

All my applications run in Docker. Having a fast CI is paramount. As Docker is not
unline a layered cake, it is important to understand how to build your images in such
a way that you can cache the layers.

- - <a href="https://keestalkstech.com/rethinking-our-asp-net-docker-ci/">Rethinking our ASP.NET Docker CI</a>
- <a href="https://keestalkstech.com/dockerfile-generator-for-net/">Dockerfile Generator for .NET</a>

### What does this project do?

This project contains a simple ASP.NET Core application that is used to demonstrate
a Dockerfile build with integration tests. To make things easier, we've created some
scripts.

### How to run it?

Easy:

```sh

# powershell
.\build.ps1

# bash
./build.sh

```

