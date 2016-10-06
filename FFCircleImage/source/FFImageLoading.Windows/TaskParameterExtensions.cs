﻿using FFImageLoading.Cache;
using FFImageLoading.Helpers;
using FFImageLoading.Work;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
#endif

namespace FFImageLoading
{
    public static class TaskParameterExtensions
    {
        /// <summary>
        /// Loads the image into given imageView using defined parameters.
        /// </summary>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static IScheduledWork Into(this TaskParameter parameters, Image imageView)
        {
            var target = new ImageTarget(imageView);
            return parameters.Into(target);
        }

        /// <summary>
        /// Loads the image into given imageView using defined parameters.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>An awaitable Task.</returns>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, Image imageView)
        {
            return parameters.IntoAsync(param => param.Into(imageView));
        }

		/// <summary>
		/// Invalidate the image corresponding to given parameters from given caches.
		/// </summary>
		/// <param name="parameters">Image parameters.</param>
		/// <param name="cacheType">Cache type.</param>
		public static async Task InvalidateAsync(this TaskParameter parameters, CacheType cacheType)
		{
			var target = new Target<WriteableBitmap, ImageLoaderTask>();
			using (var task = CreateTask(parameters, target))
			{
				var key = task.GetKey();
				await ImageService.Instance.InvalidateCacheEntryAsync(key, cacheType).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Preloads the image request into memory cache/disk cache for future use.
		/// </summary>
		/// <param name="parameters">Image parameters.</param>
		public static void Preload(this TaskParameter parameters)
		{
            if (parameters.Priority == null)
            {
                parameters.WithPriority(LoadingPriority.Low);
            }

            parameters.Preload = true;
            var target = new Target<WriteableBitmap, ImageLoaderTask>();
            var task = CreateTask(parameters, target);
            ImageService.Instance.LoadImage(task);
        }

        /// <summary>
        /// Preloads the image request into memory cache/disk cache for future use.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <param name="parameters">Image parameters.</param>
        public static Task PreloadAsync(this TaskParameter parameters)
        {
            var tcs = new TaskCompletionSource<IScheduledWork>();

            if (parameters.Priority == null)
            {
                parameters.WithPriority(LoadingPriority.Low);
            }

            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            List<Exception> exceptions = null;

            parameters.Preload = true;

            parameters
            .Error(ex =>
            {
                if (exceptions == null)
                    exceptions = new List<Exception>();

                exceptions.Add(ex);
                userErrorCallback(ex);
            })
            .Finish(scheduledWork =>
            {
                finishCallback(scheduledWork);

                if (exceptions != null)
                    tcs.TrySetException(exceptions);
                else
                    tcs.TrySetResult(scheduledWork);
            });

            var target = new Target<WriteableBitmap, ImageLoaderTask>();
            var task = CreateTask(parameters, target);
            ImageService.Instance.LoadImage(task);

            return tcs.Task;
        }

        /// <summary>
        /// Downloads the image request into disk cache for future use if not already exists.
        /// Only Url Source supported.
        /// </summary>
        /// <param name="parameters">Image parameters.</param>
        public static void DownloadOnly(this TaskParameter parameters)
        {
            if (parameters.Source == ImageSource.Url)
            {
                Preload(parameters.WithCache(CacheType.Disk));
            }
        }

        /// <summary>
        /// Downloads the image request into disk cache for future use if not already exists.
        /// Only Url Source supported.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <param name="parameters">Image parameters.</param>
        public static async Task DownloadOnlyAsync(this TaskParameter parameters)
        {
            if (parameters.Source == ImageSource.Url)
            {
                await PreloadAsync(parameters.WithCache(CacheType.Disk));
            }
        }

        private static IScheduledWork Into(this TaskParameter parameters, ITarget<WriteableBitmap, ImageLoaderTask> target)
        {
            if (parameters.Source != ImageSource.Stream && string.IsNullOrWhiteSpace(parameters.Path))
            {
                target.SetAsEmpty(null);
                parameters.Dispose();
                return null;
            }

            var task = CreateTask(parameters, target);
            ImageService.Instance.LoadImage(task);
            return task;
        }

        private static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, Action<TaskParameter> into)
        {
            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            var tcs = new TaskCompletionSource<IScheduledWork>();
            List<Exception> exceptions = null;

            parameters
                .Error(ex => {
                    if (exceptions == null)
                        exceptions = new List<Exception>();

                    exceptions.Add(ex);
                    userErrorCallback(ex);
                })
                .Finish(scheduledWork => {
                    finishCallback(scheduledWork);

                    if (exceptions != null)
                        tcs.TrySetException(exceptions);
                    else
                        tcs.TrySetResult(scheduledWork);
                });

            into(parameters);

            return tcs.Task;
        }

        private static ImageLoaderTask CreateTask(this TaskParameter parameters, ITarget<WriteableBitmap, ImageLoaderTask> target)
        {
            return new ImageLoaderTask(ImageService.Instance.Config.DownloadCache, MainThreadDispatcher.Instance, ImageService.Instance.Config.Logger, parameters, target, ImageService.Instance.Config.VerboseLoadingCancelledLogging);
        }
    }
}
