## HTTP Resilience, Kiota

My new years resolution for 2025 is not to write a custom implementation if I have an
Open API specification availabe. Now, let's test how to create a resilent setup without
any client generation, with a Kiota client.

- <a href="https://keestalkstech.com/implementing-http-resilience-by-microsoft/">Implementing HTTP Resilience by Microsoft</a>
- <a href="https://keestalkstech.com/kiota-dependency-injection-and-resilience/">Kiota, dependency injection and resilience</a>


## Checkout only this project

Do the following:

```sh
git clone --no-checkout https://github.com/KeesCBakker/keestalkstech-code-gallery.git
cd keestalkstech-code-gallery
git sparse-checkout init
git sparse-checkout set --no-cone 12.http-and-resilience
git checkout main
cd 12.http-and-resilience
ls
```
