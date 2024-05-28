$certname = "Dynamic Firewall Updater"    ## Replace {certificateName}
$expires = (Get-Date).AddYears(2)
$desktop = "$env:homedrive$env:homepath\Desktop"
$cert = New-SelfSignedCertificate -Subject "CN=$certname" -CertStoreLocation "Cert:\CurrentUser\My" -KeyExportPolicy Exportable -KeySpec Signature -KeyLength 2048 -KeyAlgorithm RSA -HashAlgorithm SHA256 -NotAfter $expires
Export-Certificate -Cert $cert -FilePath "$desktop\$certname.cer"   ## Specify your preferred location
