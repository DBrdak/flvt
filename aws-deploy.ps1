$flvtStackName = "flvt"
$flvtTemplate = ".\Flvt.API\template.yaml"
$flvtPackagedTemplate = ".\Flvt.API\packaged-budget.yaml"

$awsProfile = "dbrdak-lambda"
$s3Bucket = "flvt"
$region = "eu-west-1"

Write-Host "Packaging..."
sam build --template $flvtTemplate
sam package --s3-bucket $s3bucket --output-template-file $flvtPackagedTemplate

Write-Host "Deploying..."
sam deploy --template-file $flvtPackagedTemplate `
    --stack-name $flvtStackName `
    --capabilities CAPABILITY_IAM `
    --region $region `
    --profile $awsProfile

Write-Host "Deployment complete!" -ForegroundColor Green