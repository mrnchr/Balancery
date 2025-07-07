using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;

namespace Mrnchr.Balancery.Statistics.Database
{
  public class StatisticsDatabaseConnection : DataConnection
  {
    private readonly DbCommand _iorSession;
    private readonly DbCommand _iorTurn;
    private readonly DbCommand _iorAction;
    private readonly DbCommand _iorOption;

    public StatisticsDatabaseConnection(DataOptions dataOptions) : base(dataOptions)
    {
      this.CreateTable<SessionMetricData>(tableOptions: TableOptions.CreateIfNotExists);
      this.CreateTable<TurnMetricData>(tableOptions: TableOptions.CreateIfNotExists);
      this.CreateTable<ActionData>(tableOptions: TableOptions.CreateIfNotExists);
      this.CreateTable<StartOptionData>(tableOptions: TableOptions.CreateIfNotExists);

      _iorSession = CreateCommand();
      _iorSession.CommandText =
        "INSERT OR REPLACE INTO session_metric (session_number, metric_name, value_type, real_value, string_value) VALUES (@s, @n, @vt, @rv, @sv)";
      _iorSession.Parameters.AddRange(new[]
      {
        _iorSession.CreateParameter("@s"),
        _iorSession.CreateParameter("@n"),
        _iorSession.CreateParameter("@vt"),
        _iorSession.CreateParameter("@rv"),
        _iorSession.CreateParameter("@sv")
      });

      _iorTurn = CreateCommand();
      _iorTurn.CommandText =
        "INSERT OR REPLACE INTO turn_metric (session_number, turn_number, metric_name, value_type, real_value, string_value) VALUES (@s, @t, @n, @vt, @rv, @sv)";
      _iorTurn.Parameters.AddRange(new[]
      {
        _iorTurn.CreateParameter("@s"),
        _iorTurn.CreateParameter("@t"),
        _iorTurn.CreateParameter("@n"),
        _iorTurn.CreateParameter("@vt"),
        _iorTurn.CreateParameter("@rv"),
        _iorTurn.CreateParameter("@sv")
      });

      _iorAction = CreateCommand();
      _iorAction.CommandText =
        "INSERT OR REPLACE INTO action (session_number, turn_number, action_index, action_value) VALUES (@s, @t, @a, @v)";
      _iorAction.Parameters.AddRange(new[]
      {
        _iorAction.CreateParameter("@s"),
        _iorAction.CreateParameter("@t"),
        _iorAction.CreateParameter("@a"),
        _iorAction.CreateParameter("@v")
      });

      _iorOption = CreateCommand();
      _iorOption.CommandText =
        "INSERT OR REPLACE INTO start_option (session_number, option_name, value_type, real_value, string_value) VALUES (@s, @n, @vt, @rv, @sv)";
      _iorOption.Parameters.AddRange(new[]
      {
        _iorOption.CreateParameter("@s"),
        _iorOption.CreateParameter("@n"),
        _iorOption.CreateParameter("@vt"),
        _iorOption.CreateParameter("@rv"),
        _iorOption.CreateParameter("@sv")
      });
    }

    public ITable<SessionMetricData> SessionMetricTable => this.GetTable<SessionMetricData>();
    public ITable<TurnMetricData> TurnMetricTable => this.GetTable<TurnMetricData>();
    public ITable<ActionData> ActionTable => this.GetTable<ActionData>();
    public ITable<StartOptionData> StartOptionTable => this.GetTable<StartOptionData>();

    public void RecordSession(SessionMetricData sessionMetricData)
    {
      sessionMetricData.PrepareCommand(_iorSession);
      _iorSession.ExecuteNonQuery();
    }

    public async Task RecordSessionAsync(SessionMetricData sessionMetricData,
      CancellationToken token = default(CancellationToken))
    {
      sessionMetricData.PrepareCommand(_iorSession);
      await _iorSession.ExecuteNonQueryAsync(token);
    }

    public void RecordTurn(TurnMetricData turnMetricData)
    {
      turnMetricData.PrepareCommand(_iorTurn);
      _iorTurn.ExecuteNonQuery();
    }

    public async Task RecordTurnAsync(TurnMetricData turnMetricData,
      CancellationToken token = default(CancellationToken))
    {
      turnMetricData.PrepareCommand(_iorTurn);
      await _iorTurn.ExecuteNonQueryAsync(token);
    }

    public void RecordAction(ActionData actionData)
    {
      actionData.PrepareCommand(_iorAction);
      _iorAction.ExecuteNonQuery();
    }

    public async Task RecordActionAsync(ActionData actionData, CancellationToken token = default(CancellationToken))
    {
      actionData.PrepareCommand(_iorAction);
      await _iorAction.ExecuteNonQueryAsync(token);
    }

    public void RecordOption(StartOptionData optionData)
    {
      optionData.PrepareCommand(_iorOption);
      _iorOption.ExecuteNonQuery();
    }

    public async Task RecordOptionAsync(StartOptionData optionData,
      CancellationToken token = default(CancellationToken))
    {
      optionData.PrepareCommand(_iorOption);
      await _iorOption.ExecuteNonQueryAsync(token);
    }
  }
}