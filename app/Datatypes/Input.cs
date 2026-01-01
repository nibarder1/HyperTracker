using System.Threading.Tasks;
using Avalonia.Controls;

namespace HyperTracker.Datatypes;

/// <summary>
/// Input interface.
/// </summary>
public interface iInput
{
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
    /// Get last scanned object.
    /// </summary>
    /// <returns>Current scanned object.</returns>
    public T? GetScan<T>();
    /// <summary>
    /// Update input configuration parameters.
    /// </summary>
    /// <param name="parameters">Input parameters.</param>
    public void UpdateParameters(InputParameters parameters);
    /// <summary>
    /// Get input configuration parameter.
    /// </summary>
    /// <returns>Parameters.</returns>
    public InputParameters GetParams();
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
    /// Check if input is initialized.
    /// </summary>
    /// <returns>True if initialized.</returns>
    public bool IsInitialized();
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
    /// Build analysis control.
    /// </summary>
    /// <param name="width">Width of panel.</param>
    /// <param name="height">Height of panel.</param>
    /// <param name="rootX">Root x offset.</param>
    /// <param name="rootY">Root y offset.</param>
    /// <returns>New control.</returns>
    public Control BuildAnalysisControl(int width, int height, int rootX, int rootY);
    /// <summary>
    /// Update live feed controls.
    /// </summary>
    /// <param name="Root">Root control.</param>
    /// <returns>Updated control.</returns>
    public Control UpdateLiveControl(Control Root);
    /// <summary>
    /// Update analysis control.
    /// </summary>
    /// <param name="frame">New frame.</param>
    /// <returns>Updated control.</returns>
    public Control UpdateAnalysisControl(Avalonia.Controls.Control Root);    
    public Control BuildConfigControl(int width, int height, int rootX, int rootY);
    public Control UpdateConfigControl(Control Root);
    public void Release();
}