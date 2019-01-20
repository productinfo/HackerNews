﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Polly;
using Refit;

using HackerNews.Shared;

namespace HackerNews
{
    public static class HackerNewsAPIService
    {
        #region Constant Fields
        readonly static Lazy<IHackerNewsAPI> _hackerNewsApiClientHolder = new Lazy<IHackerNewsAPI>(() => RestService.For<IHackerNewsAPI>("https://hacker-news.firebaseio.com/v0"));

        #endregion

        #region Properties
        static IHackerNewsAPI HackerNewsApiClient => _hackerNewsApiClientHolder.Value;
        #endregion

        #region Methods
        public static Task<StoryModel> GetStory(string storyId) => ExecutePollyHttpFunction(() => HackerNewsApiClient.GetStory(storyId));
        public static Task<List<string>> GetTopStoryIDs() => ExecutePollyHttpFunction(() => HackerNewsApiClient.GetTopStoryIDs());

        static Task<T> ExecutePollyHttpFunction<T>(Func<Task<T>> action, int numRetries = 3)
        {
            return Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync
                    (
                        numRetries,
                        pollyRetryAttempt
                    ).ExecuteAsync(action);

            TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
        }
        #endregion
    }
}
