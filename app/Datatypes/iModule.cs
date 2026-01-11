using System.Threading.Tasks;
using Avalonia.Controls;

namespace HyperTracker.Datatypes;

public interface iModule
{
    
    #region Basic
    /// <summary>
    /// Check if module is initialized.
    /// </summary>
    /// <returns>True if initialized.</returns>
    public bool IsInitialized();
    /// <summary>
    /// Initialize input.
    /// </summary>
    /// <returns>True if successful.</returns>
    public bool Initialize();
    /// <summary>
    /// Start scanning.
    /// </summary>
    public Task Start();
    /// <summary>
    /// Stop scanning.
    /// </summary>
    public Task Stop();
    /// <summary>
    /// Update input configuration parameters.
    /// </summary>
    /// <param name="parameters">Input parameters.</param>
    public void UpdateParameters(InputParameters parameters);
    /// <summary>
    /// Get input configuration parameter.
    /// </summary>
    /// <returns>Parameters.</returns>
    public InputParameters? GetParams();
    /// <summary>
    /// Reinitialize input.
    /// </summary>
    /// <returns>True if successful.</returns>
    public bool ReinitializeInput();
    /// <summary>
    /// Get input type.
    /// </summary>
    /// <returns>Type of input.</returns>
    public InputTypes GetInputType();
    /// <summary>
    /// Release module resources.
    /// </summary>
    public void Release();

    #endregion

    #region  Live Feed
    /// <summary>
    /// Build live feed controls.
    /// </summary>
    /// <param name="width">Width of panel.</param>
    /// <param name="height">Height of panel.</param>
    /// <param name="rootX">Root x offset.</param>
    /// <param name="rootY">Root y offset.</param>
    /// <returns>Returns new control.</returns>
    public Control BuildLiveControl(int width, int height, int rootX, int rootY);
    /// <summary>
    /// Update live feed controls.
    /// </summary>
    /// <param name="Root">Root control.</param>
    /// <returns>Updated control.</returns>
    public Control UpdateLiveControl(Control Root);

    /// <summary>
    /// Update the live feed control internally.
    /// </summary>
    public void UpdateLiveControl();

    #endregion

    #region Analysis Panel
    /// <summary>
    /// Build analysis controls.
    /// </summary>
    /// <param name="width">Width of panel.</param>
    /// <param name="height">Height of panel.</param>
    /// <param name="rootX">Root x offset.</param>
    /// <param name="rootY">Root y offset.</param>
    /// <returns>Returns new control.</returns>
    public Control BuildAnalysisControl(int width, int height, int rootX, int rootY);
    /// <summary>
    /// Update analysis controls.
    /// </summary>
    /// <param name="Root">Root control.</param>
    /// <returns>Updated control.</returns>
    public Control UpdateAnalysisControl(Control Root);

    /// <summary>
    /// Update the analysis control internally.
    /// </summary>
    public void UpdateAnalysisControl();
    #endregion

    #region Configuration Window
    public void BuildConfigWindow(int width, int height, int rootX, int rootY);
    public void UpdateConfigWindow();
    #endregion

    #region  Calibration Window
    public void BuildCalibrationWindow(int width, int height, int rootX, int rootY);
    public void UpdateCalibrationWindow();
    #endregion


}