& {
  # development tools
  ## AWS CLI and credentials management for AWS development
  winget install Amazon.AWSCLI ByteNess.AWSVault --exact
  ## Kiro AI development environment
  winget install Amazon.Kiro --exact
  ## Docker desktop alternative and image layer inspection
  winget install SUSE.RancherDesktop wagoodman.dive --exact
  ## Build and deploy cloud-native applications
  winget install Google.ContainerTools.Skaffold --exact
  ## Git, GitHub, signing, and SSH tools
  winget install Git.Git GitHub.cli GnuPG.Gpg4win xhcoding.sshpass-win32 --exact
  ## Edit YAML and JSON files
  winget install MikeFarah.yq jqlang.jq --exact
  ## Node.js version management for Windows
  winget install CoreyButler.NVMforWindows --exact
  ## IDEs and development assistants
  winget install Microsoft.VisualStudioCode SST.opencode StablyAI.Orca --exact
  ## Compare files and directories
  winget install ScooterSoftware.BeyondCompare.5 --exact
  ## GNU-compatible command-line utilities for Windows
  winget install Microsoft.Coreutils ezwinports.make --exact

  # image and video tools
  ## Raster image and SVG editing
  winget install dotPDN.PaintDotNet Inkscape.Inkscape --exact
  ## Create diagrams
  winget install JGraph.Draw --exact
  ## Screen recording and video playback
  winget install NickeManarin.ScreenToGif VideoLAN.VLC --exact
  ## FFmpeg tools for video processing and yt-dlp
  winget install Gyan.FFmpeg yt-dlp.FFmpeg yt-dlp.yt-dlp --exact

  # tools
  ## Browse the web
  winget install Brave.Brave --exact
  ## Compare and copy files
  winget install CodeSector.TeraCopy --exact
  ## Find what is using disk space
  winget install JAMSoftware.TreeSize.Free --exact
  ## Configure Logitech peripherals
  winget install Logitech.Options --exact
  ## PowerShell and Windows productivity tools
  winget install Microsoft.PowerShell Microsoft.PowerToys --exact
  ## OpenSSL command-line toolkit for working with certificates and secrets
  winget install ShiningLight.OpenSSL.Light --exact

  # other
  ## Manage and update Ravensburger tiptoi devices
  winget install --id RavensburgerAG.tiptoiManager --exact
  ## VPN client
  winget install --id SparkLabs.Viscosity --exact

  # Microsoft Store apps
  ## Daily Bing and Windows Spotlight wallpapers
  winget install --id 9NBLGGH1ZBKW --exact --source msstore
  ## WhatsApp
  winget install --id 9NKSQGP7F2NH --exact --source msstore
  ## ChatGPT
  winget install --id 9PLM9XGG6VKS --exact --source msstore
  ## Spotify
  winget install --id 9NCBCSZSJRSB --exact --source msstore
}
