﻿// Simple OAuth .Net
// (c) 2012 Daniel McKenzie
// Simple OAuth .Net may be freely distributed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using SimpleOAuth.Utilities;

namespace SimpleOAuth
{
    /// <summary>
    /// Contains all extensions required to kick this magic off.
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// Begin signing this <see cref="WebRequest"/> object with OAuth.
        /// </summary>
        /// <param name="request">The request that needs to be signed with OAuth.</param>
        /// <returns>An <see cref="OAuthRequestWrapper"/> used to provide the required parameters for OAuth signing.</returns>
        public static OAuthRequestWrapper SignRequest(this WebRequest request)
        {
            return new OAuthRequestWrapper(request);
        }

        /// <summary>
        /// Begin signing this <see cref="WebRequest"/> object with OAuth using the tokens provided.
        /// </summary>
        /// <param name="request">The request that needs to be signed with OAuth.</param>
        /// <param name="withTokens">The <see cref="Tokens"/> to use to sign the request with.</param>
        /// <returns>An <see cref="OAuthRequestWrapper"/> used to provide the required parameters for OAuth signing.</returns>
        public static OAuthRequestWrapper SignRequest(this WebRequest request, Tokens withTokens)
        {
            return new OAuthRequestWrapper(request) { RequestTokens = withTokens };
        }

        /// <summary>
        /// For the Request and Access Token stages, makes the request and parses out the OAuth tokens from
        /// the server.
        /// </summary>
        /// <param name="request">The request that needs to be signed with OAuth.</param>
        /// <returns>A <see cref="Tokens"/> object containing the Access Token and Access Secret provided by the OAuth server.</returns>
        /// <remarks>You typically call this when making a request to get the users access tokens and combine this with the <see cref="Tokens.MergeWith"/> function.</remarks>
        /// <example>
        ///     <code>
        ///     var request = WebRequest.Create("https://api.twitter.com/oauth/request_token") { Method = "POST" };
        ///     request.SignRequest(RequestTokens)
        ///         .WithCallback("oob")
        ///         .InHeader();
        ///
        ///     var accessTokens = request.GetOAuthTokens();
        ///     RequestTokens.MergeWith(accessTokens);
        ///     </code>
        /// In the above example, the <see cref="WebRequest"/> is created, and signed with a specific set of <see cref="Tokens"/>. A call to <see cref="GetOAuthTokens"/>
        /// is made and then merged with the original Request Tokens.
        /// </example>
        /// <exception cref="WebException">Thrown when the <paramref name="request"/> encounters an error.</exception>
        public static Tokens GetOAuthTokens(this WebRequest request)
        {
            var newTokens = new Tokens();

            var output = string.Empty;
            using (var responseStream = request.GetResponse())
            {
                using (var response = new StreamReader(responseStream.GetResponseStream()))
                {
                    output = response.ReadToEnd();
                }
            }

            if (!String.IsNullOrEmpty(output))
            {
                var dataValues = UrlHelper.ParseQueryString(output);

                newTokens.AccessToken = dataValues["oauth_token"];
                newTokens.AccessTokenSecret = dataValues["oauth_token_secret"];

            }

            return newTokens;
        }

    }
}
