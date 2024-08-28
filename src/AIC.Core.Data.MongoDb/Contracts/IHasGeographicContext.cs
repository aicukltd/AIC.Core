namespace AIC.Core.Data.MongoDb.Contracts;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

public interface IHasGeographicContext<TCoordinates>
    where TCoordinates : GeoJsonCoordinates
{
    /// <summary>
    ///     Gets or sets the center.
    /// </summary>
    /// <value>
    ///     The center.
    /// </value>
    [BsonElement("Center")]
    GeoJsonPoint<TCoordinates> Center { get; set; }

    ///// <summary>
    ///// Gets or sets the top left.
    ///// </summary>
    ///// <value>
    ///// The top left.
    ///// </value>
    //GeoJsonPoint<TCoordinates> TopLeft { get;set;}

    ///// <summary>
    ///// Gets or sets the top right.
    ///// </summary>
    ///// <value>
    ///// The top right.
    ///// </value>
    //GeoJsonPoint<TCoordinates> TopRight { get;set;}

    ///// <summary>
    ///// Gets or sets the bottom left.
    ///// </summary>
    ///// <value>
    ///// The bottom left.
    ///// </value>
    //GeoJsonPoint<TCoordinates> BottomLeft { get;set;}

    ///// <summary>
    ///// Gets or sets the bottom right.
    ///// </summary>
    ///// <value>
    ///// The bottom right.
    ///// </value>
    //GeoJsonPoint<TCoordinates> BottomRight { get;set;}

    /// <summary>
    ///     Gets or sets the bounding box.
    /// </summary>
    /// <value>
    ///     The bounding box.
    /// </value>
    GeoJsonBoundingBox<TCoordinates> BoundingBox { get; set; }

    ///// <summary>
    ///// The polygon geo context.
    ///// </summary>
    //GeoJsonPolygon<TCoordinates> Polygon { get; set; }
}