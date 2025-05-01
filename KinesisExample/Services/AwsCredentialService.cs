using System.Diagnostics;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace KinesisExample.Services;

public class AwsCredentialService
{
    /*
     profile sample
     path: ~/.aws/config

     content:
     [profile {profile-name}]
     sso_start_url = {identity-center url} example: https://{...}.awsapps.com/start
     sso_region = {region}
     sso_role_name = {Permission set name}
     sso_account_id = {aws-account-id}
     region ={region}
     output = json
     */
    private readonly string _profileName = "my-sso-profile";

    public AWSCredentials GetCredentialsAsync()
    {
        return LoadSsoCredentials();
    }

    private AWSCredentials LoadSsoCredentials()
    {
        var chain = new CredentialProfileStoreChain();
        if (!chain.TryGetAWSCredentials(_profileName, out var credentials))
        {
            throw new Exception("Failed to find the my-sso-profile profile");
        }

        var ssoCredentials = credentials as SSOAWSCredentials;
        ssoCredentials.Options.ClientName = "Example-SSO-App";
        ssoCredentials.Options.SsoVerificationCallback = args =>
        {
            // Launch a browser window that prompts the SSO user to complete an SSO sign-in.
            // This method is only invoked if the session doesn't already have a valid SSO token.
            // NOTE: Process.Start might not support launching a browser on macOS or Linux. If not,
            //       use an appropriate mechanism on those systems instead.
            Process.Start(new ProcessStartInfo
            {
                FileName = args.VerificationUriComplete,
                UseShellExecute = true
            });
        };

        return ssoCredentials;
    }
}
