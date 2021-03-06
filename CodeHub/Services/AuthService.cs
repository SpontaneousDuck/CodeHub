﻿using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Credentials;
using Windows.Storage;

namespace CodeHub.Services
{
    class AuthService
    {
        #region App Creds
        GitHubClient client = new GitHubClient(new ProductHeaderValue("CodeHub"));
        Uri endUri = new Uri("http://example.com/path");
        #endregion

        /// <summary>
        /// Opens OAuth window using WebAuthenticationBroker class and returns true is authentication is successful
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Authenticate()
        {
            try
            {
                string clientId = await AppCredentials.getAppKey();
                OauthLoginRequest request = new OauthLoginRequest(clientId)
                {
                    Scopes = { "user", "repo" },
                };

                Uri oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                           WebAuthenticationOptions.None,
                                                           oauthLoginUrl,
                                                           endUri
                                                           );
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var response = WebAuthenticationResult.ResponseData;

                    var result = await Authorize(response);

                    if (result)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                    return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Makes call for Access Token
        /// </summary>
        /// <param name="response">Response string containing 'code' token, used for getting access token</param>
        /// <returns></returns>
        private async Task<bool> Authorize(string response)
        {
            try
            {
                string responseData = response.Substring(response.IndexOf("code"));
                String[] keyValPairs = responseData.Split('=');
                string code = keyValPairs[1];

                string clientId = await AppCredentials.getAppKey();
                string appSecret = await AppCredentials.getAppSecret();

                var request = new OauthTokenRequest(clientId, appSecret, code);
                var token = await client.Oauth.CreateAccessToken(request);
                if (token != null)
                {
                    client.Credentials = new Credentials(token.AccessToken);
                    SaveToken(token.AccessToken, clientId);
                }
                return true;
            }
            catch
            {
                return false;
            }



        }

        /// <summary>
        /// Saves access token in device
        /// </summary>
        /// <param name="token"></param>
        /// <param name="clientId">Client Id is used as resource string for PasswordCredential</param>
        /// <returns></returns>
        private bool SaveToken(string token, string clientId)
        {
            try
            {
                var vault = new PasswordVault();
                vault.Add(new PasswordCredential(
                   clientId, clientId, token));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets Access token if stored in device
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetToken()
        {
            try
            {
                string clientId = await AppCredentials.getAppKey();
                var vault = new PasswordVault();

                var credentialList = vault.FindAllByResource(clientId);

                if (credentialList.Count > 0)
                {
                    credentialList[0].RetrievePassword();
                    return credentialList[0].Password;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if user's device has an access token
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> checkAuth()
        {
            try
            {
                var token = await GetToken();
                    if (token != null)
                        return true;
                    else
                        return false;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Deletes the access token from user's device
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> signOut()
        {
            try
            {
                string clientId = await AppCredentials.getAppKey();
                var vault = new PasswordVault();

                var credentialList = vault.FindAllByResource(clientId);

                if (credentialList.Count > 0)
                {
                    vault.Remove(credentialList[0]);
                }

                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}
