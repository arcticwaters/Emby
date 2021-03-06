﻿using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Querying;
using ServiceStack;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Api.LiveTv
{
    /// <summary>
    /// This is insecure right now to avoid windows phone refactoring
    /// </summary>
    [Route("/LiveTv/Info", "GET", Summary = "Gets available live tv services.")]
    [Authenticated]
    public class GetLiveTvInfo : IReturn<LiveTvInfo>
    {
    }

    [Route("/LiveTv/Channels", "GET", Summary = "Gets available live tv channels.")]
    [Authenticated]
    public class GetChannels : IReturn<QueryResult<ChannelInfoDto>>
    {
        [ApiMember(Name = "Type", Description = "Optional filter by channel type.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public ChannelType? Type { get; set; }

        [ApiMember(Name = "UserId", Description = "Optional filter by user and attach user data.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string UserId { get; set; }

        /// <summary>
        /// Skips over a given number of items within the results. Use for paging.
        /// </summary>
        /// <value>The start index.</value>
        [ApiMember(Name = "StartIndex", Description = "Optional. The record index to start at. All items with a lower index will be dropped from the results.", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? StartIndex { get; set; }

        /// <summary>
        /// The maximum number of items to return
        /// </summary>
        /// <value>The limit.</value>
        [ApiMember(Name = "Limit", Description = "Optional. The maximum number of records to return", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? Limit { get; set; }

        [ApiMember(Name = "IsFavorite", Description = "Filter by channels that are favorites, or not.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool? IsFavorite { get; set; }

        [ApiMember(Name = "IsLiked", Description = "Filter by channels that are liked, or not.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool? IsLiked { get; set; }

        [ApiMember(Name = "IsDisliked", Description = "Filter by channels that are disliked, or not.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool? IsDisliked { get; set; }

        [ApiMember(Name = "EnableFavoriteSorting", Description = "Incorporate favorite and like status into channel sorting.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool EnableFavoriteSorting { get; set; }
    }

    [Route("/LiveTv/Channels/{Id}", "GET", Summary = "Gets a live tv channel")]
    [Authenticated]
    public class GetChannel : IReturn<ChannelInfoDto>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [ApiMember(Name = "Id", Description = "Channel Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }

        [ApiMember(Name = "UserId", Description = "Optional attach user data.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string UserId { get; set; }
    }

    [Route("/LiveTv/Recordings", "GET", Summary = "Gets live tv recordings")]
    [Authenticated]
    public class GetRecordings : IReturn<QueryResult<BaseItemDto>>
    {
        [ApiMember(Name = "ChannelId", Description = "Optional filter by channel id.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string ChannelId { get; set; }

        [ApiMember(Name = "UserId", Description = "Optional filter by user and attach user data.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string UserId { get; set; }

        [ApiMember(Name = "GroupId", Description = "Optional filter by recording group.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string GroupId { get; set; }

        [ApiMember(Name = "StartIndex", Description = "Optional. The record index to start at. All items with a lower index will be dropped from the results.", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? StartIndex { get; set; }

        [ApiMember(Name = "Limit", Description = "Optional. The maximum number of records to return", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? Limit { get; set; }

        [ApiMember(Name = "Status", Description = "Optional filter by recording status.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public RecordingStatus? Status { get; set; }

        [ApiMember(Name = "Status", Description = "Optional filter by recordings that are in progress, or not.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool? IsInProgress { get; set; }

        [ApiMember(Name = "SeriesTimerId", Description = "Optional filter by recordings belonging to a series timer", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string SeriesTimerId { get; set; }
    }

    [Route("/LiveTv/Recordings/Groups", "GET", Summary = "Gets live tv recording groups")]
    [Authenticated]
    public class GetRecordingGroups : IReturn<QueryResult<BaseItemDto>>
    {
        [ApiMember(Name = "UserId", Description = "Optional filter by user and attach user data.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string UserId { get; set; }
    }

    [Route("/LiveTv/Recordings/{Id}", "GET", Summary = "Gets a live tv recording")]
    [Authenticated]
    public class GetRecording : IReturn<BaseItemDto>
    {
        [ApiMember(Name = "Id", Description = "Recording Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }

        [ApiMember(Name = "UserId", Description = "Optional attach user data.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string UserId { get; set; }
    }

    [Route("/LiveTv/Tuners/{Id}/Reset", "POST", Summary = "Resets a tv tuner")]
    [Authenticated]
    public class ResetTuner : IReturnVoid
    {
        [ApiMember(Name = "Id", Description = "Tuner Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }
    }

    [Route("/LiveTv/Timers/{Id}", "GET", Summary = "Gets a live tv timer")]
    [Authenticated]
    public class GetTimer : IReturn<TimerInfoDto>
    {
        [ApiMember(Name = "Id", Description = "Timer Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }
    }

    [Route("/LiveTv/Timers/Defaults", "GET", Summary = "Gets default values for a new timer")]
    [Authenticated]
    public class GetDefaultTimer : IReturn<SeriesTimerInfoDto>
    {
        [ApiMember(Name = "ProgramId", Description = "Optional, to attach default values based on a program.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string ProgramId { get; set; }
    }

    [Route("/LiveTv/Timers", "GET", Summary = "Gets live tv timers")]
    [Authenticated]
    public class GetTimers : IReturn<QueryResult<TimerInfoDto>>
    {
        [ApiMember(Name = "ChannelId", Description = "Optional filter by channel id.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string ChannelId { get; set; }

        [ApiMember(Name = "SeriesTimerId", Description = "Optional filter by timers belonging to a series timer", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string SeriesTimerId { get; set; }
    }

    [Route("/LiveTv/Programs", "GET,POST", Summary = "Gets available live tv epgs..")]
    [Authenticated]
    public class GetPrograms : IReturn<QueryResult<BaseItemDto>>
    {
        [ApiMember(Name = "ChannelIds", Description = "The channels to return guide information for.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string ChannelIds { get; set; }

        [ApiMember(Name = "UserId", Description = "Optional filter by user id.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string UserId { get; set; }

        [ApiMember(Name = "MinStartDate", Description = "Optional. The minimum premiere date. Format = ISO", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string MinStartDate { get; set; }

        [ApiMember(Name = "HasAired", Description = "Optional. Filter by programs that have completed airing, or not.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool? HasAired { get; set; }

        [ApiMember(Name = "MaxStartDate", Description = "Optional. The maximum premiere date. Format = ISO", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string MaxStartDate { get; set; }

        [ApiMember(Name = "MinEndDate", Description = "Optional. The minimum premiere date. Format = ISO", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string MinEndDate { get; set; }

        [ApiMember(Name = "MaxEndDate", Description = "Optional. The maximum premiere date. Format = ISO", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string MaxEndDate { get; set; }

        [ApiMember(Name = "IsMovie", Description = "Optional filter for movies.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET,POST")]
        public bool? IsMovie { get; set; }

        [ApiMember(Name = "IsSports", Description = "Optional filter for sports.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET,POST")]
        public bool? IsSports { get; set; }

        [ApiMember(Name = "StartIndex", Description = "Optional. The record index to start at. All items with a lower index will be dropped from the results.", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? StartIndex { get; set; }

        [ApiMember(Name = "Limit", Description = "Optional. The maximum number of records to return", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? Limit { get; set; }

        [ApiMember(Name = "SortBy", Description = "Optional. Specify one or more sort orders, comma delimeted. Options: Name, StartDate", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET", AllowMultiple = true)]
        public string SortBy { get; set; }

        [ApiMember(Name = "SortOrder", Description = "Sort Order - Ascending,Descending", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public SortOrder? SortOrder { get; set; }

        [ApiMember(Name = "Genres", Description = "The genres to return guide information for.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string Genres { get; set; }
    }

    [Route("/LiveTv/Programs/Recommended", "GET", Summary = "Gets available live tv epgs..")]
    [Authenticated]
    public class GetRecommendedPrograms : IReturn<QueryResult<BaseItemDto>>
    {
        [ApiMember(Name = "UserId", Description = "Optional filter by user id.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string UserId { get; set; }

        [ApiMember(Name = "Limit", Description = "Optional. The maximum number of records to return", IsRequired = false, DataType = "int", ParameterType = "query", Verb = "GET")]
        public int? Limit { get; set; }

        [ApiMember(Name = "IsAiring", Description = "Optional. Filter by programs that are currently airing, or not.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool? IsAiring { get; set; }

        [ApiMember(Name = "HasAired", Description = "Optional. Filter by programs that have completed airing, or not.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool? HasAired { get; set; }

        [ApiMember(Name = "IsSports", Description = "Optional filter for sports.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET,POST")]
        public bool? IsSports { get; set; }

        [ApiMember(Name = "IsMovie", Description = "Optional filter for movies.", IsRequired = false, DataType = "bool", ParameterType = "query", Verb = "GET")]
        public bool? IsMovie { get; set; }
    }

    [Route("/LiveTv/Programs/{Id}", "GET", Summary = "Gets a live tv program")]
    [Authenticated]
    public class GetProgram : IReturn<BaseItemDto>
    {
        [ApiMember(Name = "Id", Description = "Program Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }

        [ApiMember(Name = "UserId", Description = "Optional attach user data.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string UserId { get; set; }
    }


    [Route("/LiveTv/Recordings/{Id}", "DELETE", Summary = "Deletes a live tv recording")]
    [Authenticated]
    public class DeleteRecording : IReturnVoid
    {
        [ApiMember(Name = "Id", Description = "Recording Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }
    }

    [Route("/LiveTv/Timers/{Id}", "DELETE", Summary = "Cancels a live tv timer")]
    [Authenticated]
    public class CancelTimer : IReturnVoid
    {
        [ApiMember(Name = "Id", Description = "Timer Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }
    }

    [Route("/LiveTv/Timers/{Id}", "POST", Summary = "Updates a live tv timer")]
    [Authenticated]
    public class UpdateTimer : TimerInfoDto, IReturnVoid
    {
    }

    [Route("/LiveTv/Timers", "POST", Summary = "Creates a live tv timer")]
    [Authenticated]
    public class CreateTimer : TimerInfoDto, IReturnVoid
    {
    }

    [Route("/LiveTv/SeriesTimers/{Id}", "GET", Summary = "Gets a live tv series timer")]
    [Authenticated]
    public class GetSeriesTimer : IReturn<TimerInfoDto>
    {
        [ApiMember(Name = "Id", Description = "Timer Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }
    }

    [Route("/LiveTv/SeriesTimers", "GET", Summary = "Gets live tv series timers")]
    [Authenticated]
    public class GetSeriesTimers : IReturn<QueryResult<SeriesTimerInfoDto>>
    {
        [ApiMember(Name = "SortBy", Description = "Optional. Sort by SortName or Priority", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public string SortBy { get; set; }

        [ApiMember(Name = "SortOrder", Description = "Optional. Sort in Ascending or Descending order", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET,POST")]
        public SortOrder SortOrder { get; set; }
    }

    [Route("/LiveTv/SeriesTimers/{Id}", "DELETE", Summary = "Cancels a live tv series timer")]
    [Authenticated]
    public class CancelSeriesTimer : IReturnVoid
    {
        [ApiMember(Name = "Id", Description = "Timer Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }
    }

    [Route("/LiveTv/SeriesTimers/{Id}", "POST", Summary = "Updates a live tv series timer")]
    [Authenticated]
    public class UpdateSeriesTimer : SeriesTimerInfoDto, IReturnVoid
    {
    }

    [Route("/LiveTv/SeriesTimers", "POST", Summary = "Creates a live tv series timer")]
    [Authenticated]
    public class CreateSeriesTimer : SeriesTimerInfoDto, IReturnVoid
    {
    }

    [Route("/LiveTv/Recordings/Groups/{Id}", "GET", Summary = "Gets a recording group")]
    [Authenticated]
    public class GetRecordingGroup : IReturn<BaseItemDto>
    {
        [ApiMember(Name = "Id", Description = "Recording group Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string Id { get; set; }
    }

    [Route("/LiveTv/GuideInfo", "GET", Summary = "Gets guide info")]
    [Authenticated]
    public class GetGuideInfo : IReturn<GuideInfo>
    {
    }

    [Route("/LiveTv/Folder", "GET", Summary = "Gets the users live tv folder, along with configured images")]
    [Authenticated]
    public class GetLiveTvFolder : IReturn<BaseItemDto>
    {
        [ApiMember(Name = "UserId", Description = "Optional attach user data.", IsRequired = false, DataType = "string", ParameterType = "query", Verb = "GET")]
        public string UserId { get; set; }
    }

    public class LiveTvService : BaseApiService
    {
        private readonly ILiveTvManager _liveTvManager;
        private readonly IUserManager _userManager;

        public LiveTvService(ILiveTvManager liveTvManager, IUserManager userManager)
        {
            _liveTvManager = liveTvManager;
            _userManager = userManager;
        }

        private void AssertUserCanManageLiveTv()
        {
            var user = SessionContext.GetUser(Request).Result;

            if (user == null)
            {
                throw new UnauthorizedAccessException("Anonymous live tv management is not allowed.");
            }

            if (!user.Policy.EnableLiveTvManagement)
            {
                throw new UnauthorizedAccessException("The current user does not have permission to manage live tv.");
            }
        }

        public async Task<object> Get(GetLiveTvInfo request)
        {
            var info = await _liveTvManager.GetLiveTvInfo(CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(info);
        }

        public async Task<object> Get(GetChannels request)
        {
            var result = await _liveTvManager.GetChannels(new LiveTvChannelQuery
            {
                ChannelType = request.Type,
                UserId = request.UserId,
                StartIndex = request.StartIndex,
                Limit = request.Limit,
                IsFavorite = request.IsFavorite,
                IsLiked = request.IsLiked,
                IsDisliked = request.IsDisliked,
                EnableFavoriteSorting = request.EnableFavoriteSorting

            }, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public async Task<object> Get(GetChannel request)
        {
            var user = string.IsNullOrEmpty(request.UserId) ? null : _userManager.GetUserById(request.UserId);

            var result = await _liveTvManager.GetChannel(request.Id, CancellationToken.None, user).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public async Task<object> Get(GetLiveTvFolder request)
        {
            return ToOptimizedResult(await _liveTvManager.GetLiveTvFolder(request.UserId, CancellationToken.None).ConfigureAwait(false));
        }

        public async Task<object> Get(GetPrograms request)
        {
            var query = new ProgramQuery
            {
                ChannelIds = (request.ChannelIds ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray(),
                UserId = request.UserId,
                HasAired = request.HasAired
            };

            if (!string.IsNullOrEmpty(request.MinStartDate))
            {
                query.MinStartDate = DateTime.Parse(request.MinStartDate, null, DateTimeStyles.RoundtripKind).ToUniversalTime();
            }

            if (!string.IsNullOrEmpty(request.MinEndDate))
            {
                query.MinEndDate = DateTime.Parse(request.MinEndDate, null, DateTimeStyles.RoundtripKind).ToUniversalTime();
            }

            if (!string.IsNullOrEmpty(request.MaxStartDate))
            {
                query.MaxStartDate = DateTime.Parse(request.MaxStartDate, null, DateTimeStyles.RoundtripKind).ToUniversalTime();
            }

            if (!string.IsNullOrEmpty(request.MaxEndDate))
            {
                query.MaxEndDate = DateTime.Parse(request.MaxEndDate, null, DateTimeStyles.RoundtripKind).ToUniversalTime();
            }

            query.StartIndex = request.StartIndex;
            query.Limit = request.Limit;
            query.SortBy = (request.SortBy ?? String.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            query.SortOrder = request.SortOrder;
            query.IsMovie = request.IsMovie;
            query.IsSports = request.IsSports;
            query.Genres = (request.Genres ?? String.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var result = await _liveTvManager.GetPrograms(query, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedResult(result);
        }

        public async Task<object> Get(GetRecommendedPrograms request)
        {
            var query = new RecommendedProgramQuery
            {
                UserId = request.UserId,
                IsAiring = request.IsAiring,
                Limit = request.Limit,
                HasAired = request.HasAired,
                IsMovie = request.IsMovie,
                IsSports = request.IsSports
            };

            var result = await _liveTvManager.GetRecommendedPrograms(query, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedResult(result);
        }

        public object Post(GetPrograms request)
        {
            return Get(request);
        }

        public async Task<object> Get(GetRecordings request)
        {
            var options = new DtoOptions();
            options.DeviceId = AuthorizationContext.GetAuthorizationInfo(Request).DeviceId;

            var result = await _liveTvManager.GetRecordings(new RecordingQuery
            {
                ChannelId = request.ChannelId,
                UserId = request.UserId,
                GroupId = request.GroupId,
                StartIndex = request.StartIndex,
                Limit = request.Limit,
                Status = request.Status,
                SeriesTimerId = request.SeriesTimerId,
                IsInProgress = request.IsInProgress

            }, options, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedResult(result);
        }

        public async Task<object> Get(GetRecording request)
        {
            var user = string.IsNullOrEmpty(request.UserId) ? null : _userManager.GetUserById(request.UserId);

            var options = new DtoOptions();
            options.DeviceId = AuthorizationContext.GetAuthorizationInfo(Request).DeviceId;

            var result = await _liveTvManager.GetRecording(request.Id, options, CancellationToken.None, user).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public async Task<object> Get(GetTimer request)
        {
            var result = await _liveTvManager.GetTimer(request.Id, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public async Task<object> Get(GetTimers request)
        {
            var result = await _liveTvManager.GetTimers(new TimerQuery
            {
                ChannelId = request.ChannelId,
                SeriesTimerId = request.SeriesTimerId

            }, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public void Delete(DeleteRecording request)
        {
            AssertUserCanManageLiveTv();

            var task = _liveTvManager.DeleteRecording(request.Id);

            Task.WaitAll(task);
        }

        public void Delete(CancelTimer request)
        {
            AssertUserCanManageLiveTv();

            var task = _liveTvManager.CancelTimer(request.Id);

            Task.WaitAll(task);
        }

        public void Post(UpdateTimer request)
        {
            AssertUserCanManageLiveTv();

            var task = _liveTvManager.UpdateTimer(request, CancellationToken.None);

            Task.WaitAll(task);
        }

        public async Task<object> Get(GetSeriesTimers request)
        {
            var result = await _liveTvManager.GetSeriesTimers(new SeriesTimerQuery
            {
                SortOrder = request.SortOrder,
                SortBy = request.SortBy

            }, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public async Task<object> Get(GetSeriesTimer request)
        {
            var result = await _liveTvManager.GetSeriesTimer(request.Id, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public void Delete(CancelSeriesTimer request)
        {
            AssertUserCanManageLiveTv();

            var task = _liveTvManager.CancelSeriesTimer(request.Id);

            Task.WaitAll(task);
        }

        public void Post(UpdateSeriesTimer request)
        {
            AssertUserCanManageLiveTv();

            var task = _liveTvManager.UpdateSeriesTimer(request, CancellationToken.None);

            Task.WaitAll(task);
        }

        public async Task<object> Get(GetDefaultTimer request)
        {
            if (string.IsNullOrEmpty(request.ProgramId))
            {
                var result = await _liveTvManager.GetNewTimerDefaults(CancellationToken.None).ConfigureAwait(false);

                return ToOptimizedSerializedResultUsingCache(result);
            }
            else
            {
                var result = await _liveTvManager.GetNewTimerDefaults(request.ProgramId, CancellationToken.None).ConfigureAwait(false);

                return ToOptimizedSerializedResultUsingCache(result);
            }
        }

        public async Task<object> Get(GetProgram request)
        {
            var user = string.IsNullOrEmpty(request.UserId) ? null : _userManager.GetUserById(request.UserId);

            var result = await _liveTvManager.GetProgram(request.Id, CancellationToken.None, user).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public void Post(CreateSeriesTimer request)
        {
            AssertUserCanManageLiveTv();

            var task = _liveTvManager.CreateSeriesTimer(request, CancellationToken.None);

            Task.WaitAll(task);
        }

        public void Post(CreateTimer request)
        {
            AssertUserCanManageLiveTv();

            var task = _liveTvManager.CreateTimer(request, CancellationToken.None);

            Task.WaitAll(task);
        }

        public async Task<object> Get(GetRecordingGroups request)
        {
            var result = await _liveTvManager.GetRecordingGroups(new RecordingGroupQuery
            {
                UserId = request.UserId

            }, CancellationToken.None).ConfigureAwait(false);

            return ToOptimizedSerializedResultUsingCache(result);
        }

        public async Task<object> Get(GetRecordingGroup request)
        {
            var result = await _liveTvManager.GetRecordingGroups(new RecordingGroupQuery
            {

            }, CancellationToken.None).ConfigureAwait(false);

            var group = result.Items.FirstOrDefault(i => string.Equals(i.Id, request.Id, StringComparison.OrdinalIgnoreCase));

            return ToOptimizedSerializedResultUsingCache(group);
        }

        public object Get(GetGuideInfo request)
        {
            return ToOptimizedResult(_liveTvManager.GetGuideInfo());
        }

        public void Post(ResetTuner request)
        {
            AssertUserCanManageLiveTv();

            var task = _liveTvManager.ResetTuner(request.Id, CancellationToken.None);

            Task.WaitAll(task);
        }
    }
}