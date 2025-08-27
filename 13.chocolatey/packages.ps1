& {
  # dev tools
  ## for AWS
  ## more info here: https://keestalkstech.com/2021/03/share-aws-vault-on-wsl/
  choco install -y awscli aws-vault
  ## docker, inspect docker files
  choco install -y rancher-desktop dive
  ## IDE's
  choco install -y vscode.install
  # compare
  choco install -y beyondcompare
  # FTP
  choco install -y filezilla
  ## git and git signing
  ## more info here: https://keestalkstech.com/2023/06/github-windows-ssh-gpg-devcontainer/
  choco install -y git.install gpg4win
  ## testing APIs
  choco install -y postman
  ## edit YAML files
  choco install -y yq
  ## edit JS files
  choco install -y jq
  ## Node.js
  choco install -y nvm.install
  ## NSwag Studio to generate Swagger clients
  choco install -y NSwagStudio

  # image editing tools
  ## diagrams
  choco install -y drawio
  ## for svgs:
  choco install -y InkScape
  ## better paint:
  choco install -y paint.net
  ## screen recordings
  choco install -y ffmpeg screentogif.portable

  # tools
  ## where did my diskspace go?
  choco install -y treesizefree
  ## make Windows behave better
  choco install -y powertoys
  ## PowerShell Core
  choco install -y powershell-core
  ## TeraCopy
  choco install -y teracopy
  ## OpenSSL: generate strong secrets
  choco install -y openssl
}
