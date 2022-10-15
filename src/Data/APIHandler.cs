using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using SRCStats.Models.SRC;

using static SRCStats.Models.SRC.GameApi;
using static SRCStats.Models.SRC.Records;
using static SRCStats.Models.SRC.RunApi;

namespace SRCStats.Data
{
    public class APIHandler
    {
        public static Uri BaseUri = new("https://www.speedrun.com/");

        private HttpClient _http;

        // limit for v1: 100/m
        private const int V1Limit = 5;
        private const int V1Modifier = 600;
        private static SemaphoreSlim V1Throttle = new(V1Limit);

        // limit for v2: 10/m
        private const int V2Limit = 5;
        private const int V2Modifier = 6000;
        private static SemaphoreSlim V2Throttle = new(V2Limit);

        public APIHandler()
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("User-Agent", "SRCStats");
            _http.BaseAddress = BaseUri;
            _http.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };
            _http.Timeout = TimeSpan.FromSeconds(10);
        }

        // todo: implement handling for refreshing and closing da connection
        public async Task<APIReturn> Main(string method, string? arg = null, IEnumerable<Game>? arg2 = null, IEnumerable<Run>? arg3 = null, IProgress<List<int>>? progress = null)
        {
            APIReturn result = new();
            try
            {
                // todo: make this an enum honestly
                switch (method)
                {
                    case "GetUser":
                        result.User = await GetUser(arg ?? throw new ArgumentException("Provided arg for GetUser was null!"), progress);
                        break;
                    case "GetUserStats":
                        result.UserStats = await GetUserStats(arg ?? throw new ArgumentException("Provided arg for GetUserStats was null!"), progress);
                        break;
                    case "GetUserRuns":
                        result.Runs = await GetUserRuns(arg ?? throw new ArgumentException("Provided arg for GetUserRuns was null!"), progress);
                        break;
                    case "GetRunsVerified":
                        result.Runs = await GetRunsVerified(arg ?? throw new ArgumentException("Provided arg for GetRunsVerified was null!"), progress);
                        break;
                    case "GetGamesModerated":
                        result.Games = await GetGamesModerated(arg ?? throw new ArgumentException("Provided arg for GetGamesModerated was null!"), progress);
                        break;
                    case "GetPendingRuns":
                        result.Bool = await GetPendingRuns(arg2 ?? throw new ArgumentException("Provided arg2 for GetPendingRuns was null!"), progress);
                        break;
                    case "GetGameRunCount":
                        result.Long = await GetGameRunCount(arg3 ?? throw new ArgumentException("Provided arg3 for GetGameRunCount was null!"), progress);
                        break;
                    case "GetRecords":
                        result.Records = await GetRecords(arg ?? throw new ArgumentException("Provided arg for GetRecords was null!"), arg3 ?? throw new ArgumentException("Provided arg3 for GetRecords was null!"), progress);
                        break;
                    case "GetGame":
                        result.FullGames = await GetGames(arg ?? throw new ArgumentException("Provided arg for GetGames was null!"));
                        break;
                    default:
                        throw new Exception("what the fuck");
                }
                return result;
            }
            catch
            {
                throw;
            }
            
        }

        private async Task<UserAPI?> GetUser(string username, IProgress<List<int>>? progress)
        {
            try
            {
                await SecureQueue(V1Throttle, progress);
                await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                var res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/users/{username}"));
                var model = JsonSerializer.Deserialize<UserApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                V1Throttle.Release();
                return (model ?? throw new Exception("The model generated in GetUser was null!")).Data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                V1Throttle.Release();
                return null;
            }
        }

        private async Task<Tuple<JsonNode?, JArray?>> GetUserStats(string userId, IProgress<List<int>>? progress)
        {
            try
            {
                await SecureQueue(V1Throttle, progress);
                await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                var res = await _http.GetStringAsync($"https://speedrun.com/_fedata/user/stats?userId={userId}");
                var stats = JsonSerializer.Deserialize<JsonNode>(res.ToString());
                JObject obj = JObject.Parse(res.ToString());
                var modStatsArray = (obj.GetValue("modStats") ?? throw new Exception("The token \"modStats\" wasn't found in the JObject in GetUserStats!")).Value<JArray>();
                V1Throttle.Release();
                return new Tuple<JsonNode?, JArray?>(stats, modStatsArray);
            }
            catch (Exception ex)
            {
                // todo: implement fallback to api/v2
                Debug.WriteLine(ex);
                V1Throttle.Release();
                return new Tuple<JsonNode?, JArray?>(null, null);
            }
            
        }

        private async Task<IEnumerable<RunAPI>> GetUserRuns(string userId, IProgress<List<int>>? progress = null)
        {
            var runs = new List<RunAPI>().AsEnumerable();
            try
            {
                await SecureQueue(V1Throttle, progress);
                await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                var res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/runs?user={userId}&status=verified&max=200"));
                var model = JsonSerializer.Deserialize<RunApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                runs = (model ?? throw new Exception("The model generated in GetUserRuns was null!")).Data.AsEnumerable();
                progress?.Report(new List<int> { model.Pagination.Size, model.Pagination.Size == model.Pagination.Max ? model.Pagination.Max * 2 : model.Pagination.Size });
                V1Throttle.Release();
                while (model.Pagination.Size >= model.Pagination.Max)
                {
                    await SecureQueue(V1Throttle, progress);
                    await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                    res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/runs?user={userId}&status=verified&max=200&offset={model.Pagination.Offset + 200}"));
                    model = JsonSerializer.Deserialize<RunApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    runs = runs.Concat((model ?? throw new Exception("The model generated in GetUserRuns was null!")).Data.AsEnumerable());
                    progress?.Report(new List<int> { model.Pagination.Offset + model.Pagination.Size, model.Pagination.Size == model.Pagination.Max ? model.Pagination.Offset + 400 : model.Pagination.Offset + model.Pagination.Size });
                    V1Throttle.Release();
                }
                return runs;
            }
            catch (Exception ex)
            {
                if (runs.Count() !>= 10000)
                {
                    Debug.WriteLine(ex);
                }
                progress?.Report(new List<int> { 10000, 10000 });
                V1Throttle.Release();
                return runs;
            }
        }

        private async Task<IEnumerable<RunAPI>> GetRunsVerified(string userId, IProgress<List<int>>? progress = null)
        {
            var runs = new List<RunAPI>().AsEnumerable();
            try
            {
                await SecureQueue(V1Throttle, progress);
                await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                var res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/runs?examiner={userId}&max=200"));
                var model = JsonSerializer.Deserialize<RunApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                runs = (model ?? throw new Exception("The model generated in GetRunsVerified was null!")).Data.AsEnumerable();
                progress?.Report(new List<int> { model.Pagination.Size, model.Pagination.Size == model.Pagination.Max ? model.Pagination.Max * 2 : model.Pagination.Size });
                V1Throttle.Release();
                while (model.Pagination.Size >= model.Pagination.Max)
                {
                    await SecureQueue(V1Throttle, progress);
                    await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                    res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/runs?examiner={userId}&max=200&offset={model.Pagination.Offset + 200}"));
                    model = JsonSerializer.Deserialize<RunApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    runs = runs.Concat((model ?? throw new Exception("The model generated in GetRunsVerified was null!")).Data.AsEnumerable());
                    progress?.Report(new List<int> { model.Pagination.Offset + model.Pagination.Size, model.Pagination.Size == model.Pagination.Max ? model.Pagination.Offset + 400 : model.Pagination.Offset + model.Pagination.Size });
                    V1Throttle.Release();
                }
                return runs;
            }
            catch (Exception ex)
            {
                if (runs.Count() !>= 10000)
                {
                    Debug.WriteLine(ex);
                }
                progress?.Report(new List<int> { 10000, 10000 });
                V1Throttle.Release();
                return runs;
            }
        }

        private async Task<IEnumerable<GameAPI>> GetGamesModerated(string userId, IProgress<List<int>>? progress = null)
        {
            var games = new List<GameAPI>().AsEnumerable();
            try
            {
                await SecureQueue(V1Throttle, progress);
                await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                var res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/games?moderator={userId}&embed=moderators&max=200"));
                var model = JsonSerializer.Deserialize<GameApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                games = (model ?? throw new Exception("The model generated in GetGamesModerated was null!")).Data.AsEnumerable();
                progress?.Report(new List<int> { model.Pagination.Size, model.Pagination.Size == model.Pagination.Max ? model.Pagination.Max * 2 : model.Pagination.Size });
                V1Throttle.Release();
                while (model.Pagination.Size >= model.Pagination.Max)
                {
                    await SecureQueue(V1Throttle, progress);
                    await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                    res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/games?moderator={userId}&embed=moderators&max=200&offset={model.Pagination.Offset + 200}"));
                    model = JsonSerializer.Deserialize<GameApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    games = games.Concat((model ?? throw new Exception("The model generated in GetGamesModerated was null!")).Data.AsEnumerable());
                    progress?.Report(new List<int> { model.Pagination.Offset + model.Pagination.Size, model.Pagination.Size == model.Pagination.Max ? model.Pagination.Offset + 400 : model.Pagination.Offset + model.Pagination.Size });
                    V1Throttle.Release();
                }
                return games;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                progress?.Report(new List<int> { 1, 1 });
                V1Throttle.Release();
                return games;
            }
        }

        private async Task<bool> GetPendingRuns(IEnumerable<Game> moderatedGames, IProgress<List<int>>? progress = null)
        {   
            foreach (var game in moderatedGames)
            {
                await SecureQueue(V1Throttle, progress);
                try
                {
                    await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                    var res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/runs?game={game.SiteId}&status=new"));
                    var model = JsonSerializer.Deserialize<RunApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if ((model ?? throw new Exception("The model generated in GetPendingRuns was null!")).Pagination.Size > 0)
                    {
                        progress?.Report(new List<int> { 1, 1 });
                        V1Throttle.Release();
                        return true;
                    }
                    else
                        // to ensure connection isn't closed
                        progress?.Report(new List<int> { 0, 1 });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    progress?.Report(new List<int> { 0, 1 });
                }
                V1Throttle.Release();
            }
            progress?.Report(new List<int> { 1, 1 });
            return false;
        }

        private async Task<long> GetGameRunCount(IEnumerable<Run> runs, IProgress<List<int>>? progress = null)
        {
            Dictionary<string, int> uniqueGames = new();
            long totalRunners = 0;
            foreach (var run in runs)
            {
                if (uniqueGames.ContainsKey(run.GameId))
                    uniqueGames[run.GameId] += 1;
                else
                    uniqueGames.Add(run.GameId, 1);
            }
            int i = 1;
            foreach (var game in uniqueGames)
            {
                var cst = new CancellationTokenSource();
                var ct = cst.Token;

                var v1 = Task.Run(() => GetGameRunCountV1(game, progress, ct), ct);
                var v2 = Task.Run(() => GetGameRunCountV2(game, progress, ct), ct);

                var successTask = await Task.WhenAny(v1, v2);
                cst.Cancel();
                totalRunners += successTask.Result;
                progress?.Report(new List<int> { i, uniqueGames.Count, 5 });
                i++;
            }
            return totalRunners;
        }

        private async Task<long> GetGameRunCountV1(KeyValuePair<string, int> game, IProgress<List<int>>? progress, CancellationToken ct)
        {
            try
            {
                await SecureQueue(V1Throttle, progress);
                List<string> uIds = new();
                await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier), ct);
                var res = await _http.GetStringAsync($"https://speedrun.com/api/v1/runs?game={game.Key}&status=verified&max=200");
                var model = JsonSerializer.Deserialize<RunApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                foreach (var run in (model ?? throw new Exception($"The model generated in GetGameRunCount for game {game.Key} was null!")).Data)
                    foreach (var player in run.Players)
                        if (!uIds.Contains(player.Id))
                            uIds.Add(player.Id);
                V1Throttle.Release();
                while (model?.Pagination.Size >= model?.Pagination.Max)
                {
                    await SecureQueue(V1Throttle, progress);
                    await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier), ct);
                    res = await _http.GetStringAsync($"https://speedrun.com/api/v1/runs?game={game.Key}&status=verified&max=200&offset={model?.Pagination.Offset + 200}");
                    model = JsonSerializer.Deserialize<RunApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    foreach (var run in (model ?? throw new Exception($"The model generated in GetGameRunCount for game {game.Key} was null!")).Data)
                        foreach (var player in run.Players)
                            if (!uIds.Contains(player.Id))
                                uIds.Add(player.Id);
                    V1Throttle.Release();
                }
                return uIds.Count;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == ct)
                {
                    V1Throttle.Release();
                    return game.Value;
                }
                else
                {
                    Debug.WriteLine(ex);
                    V1Throttle.Release();
                    return game.Value;
                }
            }
            // todo: if this hits an exception, then it will return first instead of letting the other method try it too, fix this?
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                V1Throttle.Release();
                return game.Value;
            }
        }

        private async Task<long> GetGameRunCountV2(KeyValuePair<string, int> game, IProgress<List<int>>? progress, CancellationToken ct)
        {
            await SecureQueue(V2Throttle, progress);
            try
            {
                await Task.Delay((int)Math.Round((double)(V2Limit - V2Throttle.CurrentCount) * V2Modifier), ct);
                var res = await _http.GetStringAsync($"https://speedrun.com/api/v2/game/getSummary?_r={FormatCall(game.Key)}");
                var stats = JsonSerializer.Deserialize<JsonElement>(res.ToString());
                var result = stats.GetProperty("gameStats").EnumerateArray().First().GetProperty("totalPlayers").GetInt32() * game.Value;
                V2Throttle.Release();
                return result;
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken == ct)
                {
                    V2Throttle.Release();
                    return 0;
                }
                else
                {
                    Debug.WriteLine(ex);
                    V2Throttle.Release();
                    return game.Value;
                }
            }
            // todo: if this hits an exception, then it will return first instead of letting the other method try it too, fix this?
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                V2Throttle.Release();
                return game.Value;
            }
        }

        // todo: could we just use this to get the game run count? look into implementing this and saving us a call
        private async Task<Dictionary<string, Runs[]>> GetRecords(string userId, IEnumerable<Run> runs, IProgress<List<int>>? progress)
        {
            Dictionary<string, int> uniqueGames = new();
            foreach (var run in runs)
            {
                if (uniqueGames.ContainsKey(run.GameId))
                    uniqueGames[run.GameId] += 1;
                else
                    uniqueGames.Add(run.GameId, 1);
            }
            var dict = new Dictionary<string, Runs[]>();
            int i = 0;
            foreach (var game in uniqueGames)
            {
                var model = new RecordData();
                try
                {
                    await SecureQueue(V1Throttle, progress);
                    await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                    var res = await _http.GetStringAsync($"https://www.speedrun.com/api/v1/games/{game.Key}/records?miscellaneous=yes&scope=all&top=10000&max=200");
                    model = JsonSerializer.Deserialize<RecordData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    V1Throttle.Release();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    V1Throttle.Release();
                    i++;
                    progress?.Report(new List<int> { i, uniqueGames.Count, 6 });
                    continue;
                }
                try
                {
                    foreach (var category in (model ?? throw new Exception($"The model generated in GetGameRunCount for game {game.Key} was null!")).Data)
                    {
                        if (uniqueGames[game.Key] < 0)
                            break;
                        bool found = false;
                        foreach (var run in category.Runs)
                        {
                            if (found)
                                break;
                            foreach (var player in run.Run.Players)
                            {
                                if (player.Id == userId)
                                {
                                    found = true;
                                    dict.Add(category.Level ?? category.Category, category.Runs);
                                    uniqueGames[game.Key] -= 1;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    continue;
                }
                i++;
                progress?.Report(new List<int> { i, uniqueGames.Count, 6});
            }
            return dict;
        }

        private async Task<IEnumerable<FullGameAPI>> GetGames(string gameName, IProgress<List<int>>? progress = null)
        {
            var games = new List<FullGameAPI>().AsEnumerable();
            try
            {
                await SecureQueue(V1Throttle, progress);
                await Task.Delay((int)Math.Round((double)(V1Limit - V1Throttle.CurrentCount) * V1Modifier));
                var res = await _http.GetStringAsync(new Uri($"https://www.speedrun.com/api/v1/games?name={gameName}&max=5&embed=categories"));
                var model = JsonSerializer.Deserialize<FullGameApiData>(res, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                games = (model ?? throw new Exception("The model generated in GetGamesModerated was null!")).Data.AsEnumerable();
                V1Throttle.Release();
                return games;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                progress?.Report(new List<int> { 1, 1 });
                V1Throttle.Release();
                return games;
            }
        }

        private static string FormatCall(string id)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"gameId\":\"" + id + "\",\"vary\":" + DateTimeOffset.Now.ToUnixTimeSeconds() + "}")).Replace('+', '-').Replace('/', '_').Replace("=", "").Replace("+", "").Replace("$", "");
        }

        private async Task SecureQueue(SemaphoreSlim queue, IProgress<List<int>>? progress)
        {
            int i = 0;
            // this gives a headstart to ones still waiting in queue compared to ones who just got out of one and want a new one
            await Task.Delay(100);
            while (!queue.Wait(TimeSpan.FromMilliseconds(100)))
            {
                i++;
                if (i >= 10)
                {
                    progress?.Report(new List<int> { -16614, 0 });
                    i = 0;
                }
            }
        }
    }
}
