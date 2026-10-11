using Microsoft.AspNetCore.Mvc;
using Oid85.FinMarket.FollowTrend.Application.Interfaces.Services;
using Oid85.FinMarket.FollowTrend.Core;
using Oid85.FinMarket.FollowTrend.Core.Requests;
using Oid85.FinMarket.FollowTrend.Core.Responses;
using Oid85.FinMarket.FollowTrend.WebHost.Controller.Base;

namespace Oid85.FinMarket.FollowTrend.WebHost.Controller;

/// <summary>
/// Тренд след.
/// </summary>
[Route("api/follow-trend")]
[ApiController]
public class FollowTrendController(
    IFollowTrendService followTrendService,
    IBacktestService backtestService)
    : BaseController
{
    /// <summary>
    /// Мониторинг версии
    /// </summary>
    [HttpPost("monitor/version")]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<MonitorResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> MonitorVersion(
        [FromBody] MonitorRequest request) =>
        GetResponseAsync(
            () => followTrendService.MonitorVersionAsync(request),
            result => new BaseResponse<MonitorResponse> { Result = result });

    /// <summary>
    /// Терминал версии
    /// </summary>
    [HttpPost("terminal/version")]
    [ProducesResponseType(typeof(BaseResponse<TerminalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<TerminalResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<TerminalResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> TerminalVersion(
        [FromBody] TerminalRequest request) =>
        GetResponseAsync(
            () => followTrendService.TerminalAsync(request),
            result => new BaseResponse<TerminalResponse> { Result = result });

    /// <summary>
    /// Выполнить бэктест
    /// </summary>
    [HttpPost("backtest")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> Backtest(
        [FromBody] BacktestRequest request) =>
        GetResponseAsync(
            () => backtestService.BacktestAsync(request),
            result => new BaseResponse<BacktestResponse> { Result = result });

    /// <summary>
    /// Результаты бектеста
    /// </summary>
    [HttpPost("backtest/result")]
    [ProducesResponseType(typeof(BaseResponse<BacktestResultResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResultResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<BacktestResultResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> BacktestResult(
        [FromBody] BacktestResultRequest request) =>
        GetResponseAsync(
            () => backtestService.BacktestResultAsync(request),
            result => new BaseResponse<BacktestResultResponse> { Result = result });

    /// <summary>
    /// Редактировать сумму портфеля
    /// </summary>
    [HttpPost("portfolio/total-sum/edit")]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<EditPortfolioTotalSumResponse>), StatusCodes.Status500InternalServerError)]
    public Task<IActionResult> EditPortfolioTotalSumAsync(
        [FromBody] EditPortfolioTotalSumRequest request) =>
        GetResponseAsync(
            () => followTrendService.EditPortfolioTotalSumAsync(request),
            result => new BaseResponse<EditPortfolioTotalSumResponse> { Result = result });
}