& {
  # download and use node 22 LTS
  nvm install 22
  nvm use 22
}

& {
  # check if your package file is still up to date
  npm install --global npm-check-updates
}
