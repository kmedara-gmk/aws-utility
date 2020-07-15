using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSUtility.Credentials
{
    class ProfileClient
    {
        /// <summary>
        /// Retrieves specified profile info from default location of aws credentials file in local user dir
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public AWSCredentials GetCredentialsByProfileName(string profileName)
        {
            var chain = new CredentialProfileStoreChain();
            Amazon.Runtime.AWSCredentials credentials;
            if (chain.TryGetAWSCredentials(profileName, out credentials))
            {
                SharedCredentialsFile credentialFile = new SharedCredentialsFile();
                return credentials;
            }
            return null;
        
    }
}
}
