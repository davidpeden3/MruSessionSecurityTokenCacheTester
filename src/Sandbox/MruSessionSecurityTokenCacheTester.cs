using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;

using NUnit.Framework;

namespace Sandbox
{
	[TestFixture]
	public class MruSessionSecurityTokenCacheTester
	{
		private readonly ClaimsPrincipal principal = new ClaimsPrincipal();
		private readonly DateTime expiryTime = DateTime.UtcNow.AddDays(10);
		private readonly SessionSecurityTokenCache tokenCache = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.Caches.SessionSecurityTokenCache;
		private const string context = "context";
		private string endpointId;

		[Test]
		public void ExpectedFailure()
		{
			endpointId = null;
			var token = new SessionSecurityToken(principal, context, endpointId, DateTime.UtcNow, expiryTime); //System.ArgumentNullException : Value cannot be null. Parameter name: endpointId
			SessionSecurityTokenCacheKey cacheKey = CacheToken(token);
			AssertGet(cacheKey, token);
			AssertGetAll(cacheKey, token);
		}

		[Test]
		public void UnexpectedFailure_Explicit()
		{
			endpointId = String.Empty;
			var token = new SessionSecurityToken(principal, context, endpointId, DateTime.UtcNow, expiryTime);
			SessionSecurityTokenCacheKey cacheKey = CacheToken(token);
			AssertGet(cacheKey, token);
			AssertGetAll(cacheKey, token); //System.InvalidOperationException : Sequence contains no elements
		}

		[Test]
		public void UnexpectedFailure_Implicit()
		{
			var token = new SessionSecurityToken(principal);
			SessionSecurityTokenCacheKey cacheKey = CacheToken(token);
			AssertGet(cacheKey, token);
			AssertGetAll(cacheKey, token); //System.InvalidOperationException : Sequence contains no elements
		}

		[Test]
		public void Passes()
		{
			endpointId = "endointId";
			var token = new SessionSecurityToken(principal, context, endpointId, DateTime.UtcNow, expiryTime);
			SessionSecurityTokenCacheKey cacheKey = CacheToken(token);
			AssertGet(cacheKey, token);
			AssertGetAll(cacheKey, token);
		}

		private SessionSecurityTokenCacheKey CacheToken(SessionSecurityToken token)
		{
			var cacheKey = new SessionSecurityTokenCacheKey(token.EndpointId, token.ContextId, token.KeyGeneration);
			tokenCache.AddOrUpdate(cacheKey, token, expiryTime);
			return cacheKey;
		}

		private void AssertGet(SessionSecurityTokenCacheKey cacheKey, SessionSecurityToken expectedToken)
		{
			SessionSecurityToken fetchedToken = tokenCache.Get(cacheKey);
			Assert.That(fetchedToken, Is.EqualTo(expectedToken));
		}

		private void AssertGetAll(SessionSecurityTokenCacheKey cacheKey, SessionSecurityToken expectedToken)
		{
			IEnumerable<SessionSecurityToken> fetchedTokens = tokenCache.GetAll(cacheKey.EndpointId, cacheKey.ContextId);
			Assert.That(fetchedTokens.Single(), Is.EqualTo(expectedToken));
		}
	}
}