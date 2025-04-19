# KeesTalksTech Gallery Repository

This repository contains code samples for articles on my blog.
It is easier to keep the code up to date when all of the items are grouped.

## 1. Dependency injection (with IOptions) in Console Apps in .NET

When you are used to building web applications, you kind of get hooked to the
ease of Dependency Injection (DI) and the way settings can be specified in a
JSON file and accessed through DI (``IOptions<T>``). It's only logical to
want the same features in your Console app.

- <a href="01.dependency-injection-with-ioptions-in-console-apps">01.dependency-injection-with-ioptions-in-console-apps</a>
- <a href="https://keestalkstech.com/2018/04/dependency-injection-with-ioptions-in-console-apps-in-dotnet/">Dependency injection (with IOptions) in Console Apps in .NET</a>


## 2. Simple JWT Access Policies for API security in .NET

Services can use their private key to communicate with our service.
We can configure the access for each issuer using standard .NET claims.

- <a href="02.simple-jwt-access-policies-for-api-security-in-net">02.simple-jwt-access-policies-for-api-security-in-net</a>
- <a href="https://keestalkstech.com/debug-a-jwt-token-expiration-locally-with-bash-or-powershell/">Debug a JWT token expiration locally with bash or PowerShell</a>
- <a href="https://keestalkstech.com/2024/11/simple-jwt-access-policies-for-api-security-in-net/">Simple JWT Access Policies for API security in .NET</a>

## 3. Options Injection

Options themselves can be collections as well. This
project shows how to use a `Dictionary<string, string>` and a
`List<string>` as base classes for options. Both are supported
by default.

- <a href="03.option-injection">03.option-injection</a>

## 4. .NET Console Application with injectable commands (System.CommandLine preview)

How to use `System.CommandLine` to build a CLI with commands and
dependency injection.

- <a href="04.command-line-di-poc">04.command-line-di-poc</a>

## 5. Roman Numerals
Parsing Roman Numerals in C# is a good way to explore
(implicit) operator overloading.

- <a href="05.roman-numerals">05.roman-numerals</a>
- <a href="https://keestalkstech.com/2017/08/parsing-roman-numerals-using-csharp/">Parsing Roman Numerals using C#</a>
- <a href="https://keestalkstech.com/2017/08/calculations-with-roman-numerals-in-csharp/">Calculations with Roman Numerals using C#</a>

## 6. Handlebars.Net & JSON templates

I ❤️ Handlebars! So I was very very very happy to see that Handlebars was ported to .NET!
It is a mega flexible templating engine as it can easily be extended. I'm working on a
project where I need to parse objects via JSON templates to JSON strings. This blog will
show how to instruct Handlebars to parse into JSON and add some nice error messages
if your template fails.

- <a href="06.handlebars">06.handlebars</a>
- <a href="https://keestalkstech.com/2022/09/handlebars-net-json-templates/">Handlebars.Net & JSON templates</a>
- <a href="https://keestalkstech.com/2022/09/handlebars-net-fun-with-flags/">Handlebars.Net: Fun with [Flags]</a>

## 7. Docker

All my applications run in Docker. Having a fast CI is paramount. As Docker is not
unline a layered cake, it is important to understand how to build your images in such
a way that you can cache the layers.

- <a href="07.docker">07.docker</a>
- <a href="https://keestalkstech.com/rethinking-our-asp-net-docker-ci/">Rethinking our ASP.NET Docker CI</a>
- <a href="https://keestalkstech.com/dockerfile-generator-for-net/">Dockerfile Generator for .NET</a>

## 8. Bypass Company Wallpaper as Local Admin

Looks like my organization wants to manage my background picture. But I'm a local admin,
so let's see if we can still change that background picture. When I go
to Setting > Personalization > Background, I see the message:
"Some of these settings are managed by your organization." If you are an admin,
you might be able to (temporarily) override the wallpaper, by editing your registry and
triggering a refresh of the wallpaper. If you can, you can automate it using PowerShell.

- <a href="08.bypass-company-wallpaper">08.bypass-company-wallpaper</a>
- <a href="https://keestalkstech.com/2024/12/how-to-bypass-company-background-image-as-local-admin-on-windows-11/">How to bypass company wallpaper as local admin on Windows 11</a>

## 9. Simple Python code to send messages to a Slack channel (without packages)

The Slack API is so simple, that you don't really need a package to post a simple message
to a Slack channel. This simple Python code will show you how!

- <a href="09.simple-slack-messages">09.simple-slack-messages</a>
- <a href="https://keestalkstech.com/simple-python-code-to-send-message-to-slack-channel-without-packages/">Simple Python code to send messages to a Slack channel (without packages)</a>


## 11. Jupyter Notebook and PIL images

When working with images in a Python notebook I like to visualize them on a grid. Just calling
`display` is not enough as it renders the images underneath each other. Let's use Matplotlib to
generate a grid of PIL images.


- <a href="11.jupyter-pil-images">11.jupyter-pil-images</a>
- <a href="https://keestalkstech.com/plotting-a-grid-of-pil-images-in-jupyter/">Plotting a grid of PIL images in Jupyter</a>


## 12. HTTP Resilience, Kiota

My new years resolution for 2025 is not to write a custom implementation if I have an
Open API specification availabe. Now, let's test how to create a resilent setup without
any client generation, with a Kiota client.

- <a href="12.http-and-resilience">12.http-and-resilience</a>
- <a href="https://keestalkstech.com/implementing-http-resilience-by-microsoft/">Implementing HTTP Resilience by Microsoft</a>
- <a href="https://keestalkstech.com/kiota-dependency-injection-and-resilience/">Kiota, dependency injection and resilience</a>

## 13. Chocolatey
With the <a href="https://community.chocolatey.org/">Chocolatey Package Manager</a> for Windows, it is super easy to install software from the command-line. This makes your installs scriptable and thus repeatable. In this article, I'll show you how to render installation instructions from a machine and how to use the Windows Task Scheduler to update your packages regularly.

- <a href="13.chocolatey">12.kiota-and-resilience</a>
- <a href="https://keestalkstech.com/notes-on-chocolatey/">Notes on Chocolatey</a>

## 14. Validation
Let's explore validation using .NET Data Annotations and FluentValidation.

- <a href="14.validation">14.validation</a>
- <a href="https://keestalkstech.com/lets-combine-data-annotation-validation-with-fluentvalidation/">Let’s combine Data Annotation Validation with FluentValidation</a>
- <a href="https://keestalkstech.com/data-annotation-validation-in-a-business-service/">Data Annotation validation with dependency injection in a business service</a>
- <a href="https://keestalkstech.com/is-one-of-and-is-not-one-of-validation-attributes/">“Is One Of” and “Is Not One Of” validation attributes</a>
