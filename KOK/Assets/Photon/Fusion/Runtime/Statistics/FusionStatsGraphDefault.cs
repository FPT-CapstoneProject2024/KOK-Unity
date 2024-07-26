namespace Fusion.Statistics
{
using UnityEngine;
using UnityEngine.UI;
  public class FusionStatsGraphDefault : FusionStatsGraphBase
    {
        internal RenderSimStats Stat => _selectedStats;
        private RenderSimStats _selectedStats;
        [SerializeField] private Text _descriptionText;

        protected override void Initialize() {
          base.Initialize();
          _descriptionText.text = _selectedStats.ToString();
        }

        public override void UpdateGraph(NetworkRunner runner, FusionStatisticsManager statisticsManager) {
          AddValueToBuffer(GetStatData(_selectedStats, statisticsManager.CompleteSnapshot));
        }

        internal void SetupDefaultGraph(RenderSimStats stat) {
          _selectedStats = stat;
          
          string valueTextFormat;
          float valueTextMultiplier = 1;
          bool ignoreZeroOnAverage = false, ignoreZeroOnBuffer = false;
          
          switch (stat) {
            case RenderSimStats.InPackets:
            case RenderSimStats.OutPackets:
            case RenderSimStats.InObjectUpdates:
            case RenderSimStats.OutObjectUpdates:
              valueTextFormat = "{0:0}";
              break;
            
            case RenderSimStats.RTT:
              valueTextFormat = "{0:0} ms";
              valueTextMultiplier = 1000;
              ignoreZeroOnAverage = true; ignoreZeroOnBuffer = true;
              break;
            
            case RenderSimStats.InBandwidth:
            case RenderSimStats.OutBandwidth:
            case RenderSimStats.InputInBandwidth:
            case RenderSimStats.InputOutBandwidth:
            case RenderSimStats.AverageInPacketSize:
            case RenderSimStats.AverageOutPacketSize:
              valueTextFormat = "{0:0} B";
              break;
            
            case RenderSimStats.Resimulations:
              valueTextFormat = "{0:0}";
              break;
            case RenderSimStats.ForwardTicks:
              valueTextFormat = "{0:0}";
              break;
            
            case RenderSimStats.TimeResets:
            case RenderSimStats.SimulationSpeed:
            case RenderSimStats.InterpolationSpeed:
              valueTextFormat = "{0:0}";
              break;
            
            // All time stats are normalized to use seconds, so 1000 multiplier to be ms.
            case RenderSimStats.InputReceiveDelta:
            case RenderSimStats.StateReceiveDelta:
            case RenderSimStats.SimulationTimeOffset:
            case RenderSimStats.InterpolationOffset:
              valueTextMultiplier = 1000;
              valueTextFormat = "{0:0} ms";
              break;
            
            case RenderSimStats.GeneralAllocatedMemoryInUse:
            case RenderSimStats.ObjectsAllocatedMemoryInUse:
              valueTextFormat = "{0:0} B";
              break;
            
            default:
              valueTextFormat = "{0:0}";
              break;
          }
          
          SetValueTextFormat(valueTextFormat);
          SetValueTextMultiplier(valueTextMultiplier);
          SetIgnoreZeroValues(ignoreZeroOnAverage, ignoreZeroOnBuffer);
          Initialize();
        }

        private float GetStatData(RenderSimStats statID, FusionStatisticsSnapshot simulationStatsSnapshot) {
          switch (statID) {
            // Sim stats
            case RenderSimStats.InPackets:
              return simulationStatsSnapshot.InPackets;
            case RenderSimStats.OutPackets:
              return simulationStatsSnapshot.OutPackets;
            case RenderSimStats.RTT:
              return simulationStatsSnapshot.RoundTripTime;
            case RenderSimStats.InBandwidth:
              return simulationStatsSnapshot.InBandwidth;
            case RenderSimStats.OutBandwidth:
              return simulationStatsSnapshot.OutBandwidth;
            case RenderSimStats.Resimulations:
              return simulationStatsSnapshot.Resimulations;
            case RenderSimStats.ForwardTicks:
              return simulationStatsSnapshot.ForwardTicks;
            case RenderSimStats.InputInBandwidth:
              return simulationStatsSnapshot.InputInBandwidth;
            case RenderSimStats.InputOutBandwidth:
              return simulationStatsSnapshot.InputOutBandwidth;
            case RenderSimStats.AverageInPacketSize:
              return simulationStatsSnapshot.InBandwidth / Mathf.Max(simulationStatsSnapshot.InPackets, 1);
            case RenderSimStats.AverageOutPacketSize:
              return simulationStatsSnapshot.OutBandwidth / Mathf.Max(simulationStatsSnapshot.OutPackets, 1);
            case RenderSimStats.InObjectUpdates:
              return simulationStatsSnapshot.InObjectUpdates;
            case RenderSimStats.OutObjectUpdates:
              return simulationStatsSnapshot.OutObjectUpdates;
            case RenderSimStats.ObjectsAllocatedMemoryInUse:
              return simulationStatsSnapshot.ObjectsAllocMemoryUsedInBytes;
            case RenderSimStats.GeneralAllocatedMemoryInUse:
              return simulationStatsSnapshot.GeneralAllocMemoryUsedInBytes;
            
            // Time stats
            case RenderSimStats.InputReceiveDelta:
              return simulationStatsSnapshot.InputReceiveDelta;
            case RenderSimStats.TimeResets:
              return simulationStatsSnapshot.TimeResets;
            case RenderSimStats.StateReceiveDelta:
              return simulationStatsSnapshot.StateReceiveDelta;
            case RenderSimStats.SimulationTimeOffset:
              return simulationStatsSnapshot.SimulationTimeOffset;
            case RenderSimStats.SimulationSpeed:
              return simulationStatsSnapshot.SimulationSpeed;
            case RenderSimStats.InterpolationOffset:
              return simulationStatsSnapshot.InterpolationOffset;
            case RenderSimStats.InterpolationSpeed:
              return simulationStatsSnapshot.InterpolationSpeed;
          }
          
          return default;
        }
    }
}
