# remove pin, install specific version and pin again
choco pin remove -n git.install
choco install git.install --version=2.46.2
choco pin add -n git.install

# list pins
choco pin list
