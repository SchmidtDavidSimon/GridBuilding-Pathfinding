namespace Utilities
{
    /// <summary>
    /// An enumerator describing the different Movements allowed on this cell
    /// </summary>
    public enum MovementType
    {
        Cars,
        Pedestrians
    }
    /// <summary>
    /// An enumerator describing the different types of contents that can exist on a cell, with None being the default and OutOfBounds being a helper for some methods. 
    /// </summary>
    public enum CellContentType
    {
        Street,
        Residence1,
        Residence2,
        Special1,
        Vegetation, 
        OutOfBounds,
        None = -1
    }
}