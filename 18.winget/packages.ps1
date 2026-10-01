& {
  # development tools
  winget install --id Amazon.AWSCLI --exact
  winget install --id Amazon.Kiro --exact
  ## Node Version Manager for Windows
  winget install --id CoreyButler.NVMforWindows --exact
  winget install --id Git.Git --exact
  winget install --id GitHub.cli --exact
  winget install --id GnuPG.Gpg4win --exact
  winget install --id Google.ContainerTools.Skaffold --exact
  winget install --id jqlang.jq --exact
  winget install --id Microsoft.VisualStudioCode --exact
  winget install --id MikeFarah.yq --exact
  winget install --id ScooterSoftware.BeyondCompare.5 --exact
  winget install --id SST.opencode --exact
  winget install --id SUSE.RancherDesktop --exact
  winget install --id wagoodman.dive --exact
  winget install --id xhcoding.sshpass-win32 --exact

  # image and video tools
  winget install --id dotPDN.PaintDotNet --exact
  winget install --id Gyan.FFmpeg --exact
  winget install --id Inkscape.Inkscape --exact
  ## draw.io
  winget install --id JGraph.Draw --exact
  winget install --id NickeManarin.ScreenToGif --exact
  winget install --id VideoLAN.VLC --exact
  winget install --id yt-dlp.FFmpeg --exact
  winget install --id yt-dlp.yt-dlp --exact

  # tools
  winget install --id Brave.Brave --exact
  winget install --id CodeSector.TeraCopy --exact
  winget install --id JAMSoftware.TreeSize.Free --exact
  winget install --id Logitech.Options --exact
  winget install --id Microsoft.Coreutils --exact
  winget install --id Microsoft.PowerShell --exact
  winget install --id Microsoft.PowerToys --exact
  winget install --id RavensburgerAG.tiptoiManager --exact
  ## OpenSSL command-line toolkit
  winget install --id ShiningLight.OpenSSL.Light --exact
  winget install --id SparkLabs.Viscosity --exact
  winget install --id StablyAI.Orca --exact

  # Microsoft Store apps
  ## Dynamic Theme: daily Bing and Windows Spotlight wallpapers
  winget install --id 9NBLGGH1ZBKW --exact --source msstore
  ## WhatsApp
  winget install --id 9NKSQGP7F2NH --exact --source msstore
  ## ChatGPT
  winget install --id 9PLM9XGG6VKS --exact --source msstore
  ## Spotify
  winget install --id 9NCBCSZSJRSB --exact --source msstore
}
