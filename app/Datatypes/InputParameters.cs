using System.Collections.Generic;

namespace HyperTracker.Datatypes;

/// <summary>
/// Input parameters.
/// </summary>
public class InputParameters
{
    /// <summary>
    /// Parameters.
    /// </summary>
    private Dictionary<string, object> _Params;

    public bool HasKey(string key)
    {
        return this._Params.ContainsKey(key);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public InputParameters()
    {
        _Params = new Dictionary<string, object>();
    }

    /// <summary>
    /// Add a parameter to the input or update if key already exists.
    /// </summary>
    /// <param name="key">Parameter key.</param>
    /// <param name="value">Parameter value.</param>
    public void AddParam(string key, object value)
    {
        if(_Params.ContainsKey(key))
        {
            _Params[key] = value;
        }else
        {
            _Params.Add(key, value);
        }
    }

    /// <summary>
    /// Get parameter value.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="key">Parameter key.</param>
    /// <returns>Parameter value.</returns>
    public T? GetParameter<T>(string key)
    {
        if(!this._Params.ContainsKey(key))
        {
            return default(T);
        }
        return (T)this._Params[key];
    }
}