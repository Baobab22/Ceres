﻿#region License notice

/*
  This file is part of the Ceres project at https://github.com/dje-dev/ceres.
  Copyright (C) 2020- by David Elliott and the Ceres Authors.

  Ceres is free software under the terms of the GNU General Public License v3.0.
  You should have received a copy of the GNU General Public License
  along with Ceres. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

#region Using directives

#endregion

using System.ComponentModel.DataAnnotations;

namespace Ceres.Features.UCI
{
  public partial class UCIManager
  {
    /// <summary>
    /// Optional text file to receive diagnostic log relating to Ceres engine moves.
    /// </summary>
    string logFileName = null;

    /// <summary>
    /// If verbose move stats should be output at periodic intervals.
    /// </summary>
    bool logLiveStats = false;

    /// <summary>
    /// If detailed top-level move info should be output at end of move selection.
    /// </summary>
    bool verboseMoveStats = false;

    /// <summary>
    /// If the value head score should be output as a logistic (probability of winning),
    /// otherwise output as centipawn equivalent.
    /// </summary>
    bool scoreAsQ = false;

    /// <summary>
    /// If the W/D/L scores should each be shown in UCI info lines
    /// </summary>
    bool showWDL = false;

    /// <summary>
    /// If MultiPV info lines use N for child move.
    /// </summary>
    bool perPVCounters = false;

    /// <summary>
    /// Number of moves for which to output PVs in UCI input (multiPV mode is where numPV > 1).
    /// </summary>
    int numPV = 1;


    void ProcessSetOption(string command)
    {
      string[] parts = command.Split(" ");

      if (parts.Length < 4
       || parts[0].ToLower() != "setoption"
       || parts[1].ToLower() != "name"
       || parts[3].ToLower() != "value"
       )
      {
        OutStream.WriteLine("Expected command of form setoption name <option_name> value <option_value>");
        return;
      }

      string name = parts[2];
      string value = parts.Length < 5 ? "" : parts[4];

      switch (name.ToLower())
      {
        case "logfile":
          logFileName = value == "" ? null : value;
          if (CeresEngine != null) CeresEngine.LogFileName = logFileName;
          break;

        case "loglivestats":
          SetBool(value, ref logLiveStats);
          break;

        case "smartpruningfactor":
          float factor = -1;
          SetFloat(value, 0, int.MaxValue, ref factor);
          if (factor == 0)
            ParamsSearch.FutilityPruningStopSearchEnabled = false;
          else
            throw new System.Exception("Ceres only supports value 0 for SmartPruningFactor");
          break;

        case "verbosemovestats":
          SetBool(value, ref verboseMoveStats);
          break;

        case "scoretype":
          value = value.ToLower();
          if (value == "centipawn")
            scoreAsQ = false;
          else if (value == "w-l" || value == "q")
            scoreAsQ = true;
          else
            OutStream.Write("Invalid value for ScoreType, allowable values are Centipawn, Q or W-L");
          break;

          //option name ScoreType type combo default centipawn var centipawn var Q var W-L

        case "multipv":
          SetInt(value, 1, int.MaxValue, ref numPV);
          break;

        case "perpvcounters":
          SetBool(value, ref perPVCounters);
          break;

        case "uci_showwdl":
          SetBool(value, ref showWDL);
          break;

      }

      //setoption name VerboseMoveStats value true
      //setoption name LogLiveStats value true

    }

    void SetBool(string boolStr, ref bool value)
    {
      if (boolStr.ToLower() == "true")
        value = true;
      else if (boolStr.ToLower() == "false")
        value = false;
      else
        OutStream.WriteLine("Invalid value, expected true or false");
    }


    void SetInt(string intStr, int minValue, int maxValue, ref int value)
    {
      if (!int.TryParse(intStr, out int newValue))
      {
        OutStream.WriteLine("Invalid value, expected integer");
      }

      if (newValue < minValue)
      {
        OutStream.WriteLine($"Value below minimum of {minValue}");
      }
      else if (newValue > maxValue)
      {
        OutStream.WriteLine($"Value above maximum of {maxValue}");
      }
      else
        value = newValue;
    }

    void SetFloat(string intStr, float minValue, float maxValue, ref float value)
    {
      if (!float.TryParse(intStr, out float newValue))
      {
        OutStream.WriteLine("Invalid value, expected number");
      }

      if (newValue < minValue)
      {
        OutStream.WriteLine($"Value below minimum of {minValue}");
      }
      else if (newValue > maxValue)
      {
        OutStream.WriteLine($"Value above maximum of {maxValue}");
      }
      else
        value = newValue;
    }

    const string SetOptionUCIDescriptions =
@"
option name LogFile type string default
option name MultiPV type spin default 1 min 1 max 500
option name VerboseMoveStats type check default false
option name LogLiveStats type check default false
option name SmartPruningFactor type string default 1
option name PerPVCounters type check default false
option name ScoreType type combo default centipawn var centipawn var Q var W-L
";
    /*
option name SyzygyPath type string default
option name UCI_ShowWDL type check default false
option name ConfigFile type string default lc0.config

option name HistoryFill type combo default fen_only var no var fen_only var always
option name CPuct type string default 2.147000
option name CPuctAtRoot type string default 2.147000
option name CPuctBase type string default 18368.000000
option name CPuctBaseAtRoot type string default 18368.000000
option name CPuctFactor type string default 2.815000
option name CPuctFactorAtRoot type string default 2.815000
option name RamLimitMb type spin default 0 min 0 max 100000000
option name MoveOverheadMs type spin default 200 min 0 max 100000000
";
*/

  }

}