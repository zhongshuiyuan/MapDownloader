﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMapCommonType;
using GMap.NET;

namespace GMapProvidersExt.Baidu
{
    public class BaiduProjection : PureProjection
    {
        // Fields
        public static readonly BaiduProjection Instance;
        private static readonly double MaxLatitude;
        private static readonly double MaxLongitude;
        private static readonly double MinLatitude;
        private static readonly double MinLongitude;
        private readonly GSize tileSize;

        // Methods
        static BaiduProjection()
        {
            Instance = new BaiduProjection();
            MinLatitude = -85.05112878;
            MaxLatitude = 85.05112878;
            MinLongitude = -180.0;
            MaxLongitude = 180.0;
        }

        private BaiduProjection()
        {
            this.tileSize = new GSize(0x100, 0x100);
        }

        public override GPoint FromLatLngToPixel(double lat, double lng, int zoom)
        {
            //Point2D pointd = LonLat2Mercator(new Point2D(lng, lat));
            PointLatLng p = LonLat2Mercator(lng, lat);
            p.Lat -= 20000.0;
            this.GetTileMatrixMinXY(zoom);
            double levelResolution = this.GetLevelResolution(zoom);
            long x = (long)((p.Lng - this.BaiduProjectedOrigin.Lng) / levelResolution);
            return new GPoint(x, (long)((this.BaiduProjectedOrigin.Lat - p.Lat) / levelResolution));
        }

        public override PointLatLng FromPixelToLatLng(long x, long y, int zoom)
        {
            double levelResolution = this.GetLevelResolution(zoom);
            double num2 = x * levelResolution;
            double num3 = y * levelResolution;
            this.GetTileMatrixMinXY(zoom);
            double num4 = num2 + this.BaiduProjectedOrigin.Lng;
            double num5 = this.BaiduProjectedOrigin.Lat - num3;
            num5 += 20000.0;
            PointLatLng lng = this.MercatorToLonLat(num4, num5);
            if (lng.Lat < MinLatitude)
            {
                lng.Lat = MinLatitude;
            }
            if (lng.Lat > MaxLatitude)
            {
                lng.Lat = MaxLatitude;
            }
            if (lng.Lng < MinLongitude)
            {
                lng.Lng = MinLongitude;
            }
            if (lng.Lng > MaxLongitude)
            {
                lng.Lng = MaxLongitude;
            }
            return lng;
        }

        public override double GetGroundResolution(int zoom, double latitude)
        {
            return this.GetLevelResolution(zoom);
        }

        public double GetLevelResolution(int level)
        {
            return Math.Pow(2.0, (double)(0x12 - level));
        }

        public override GSize GetTileMatrixMaxXY(int zoom)
        {
            return GetTile(this.BaiduProjectedOrigin.Lng, this.BaiduProjectedOrigin.Lat, this.ProjectedBounds.Right, this.ProjectedBounds.Bottom, this.GetLevelResolution(zoom), 0x100, 0x100);
        }

        public override GSize GetTileMatrixMinXY(int zoom)
        {
            return GetTile(this.BaiduProjectedOrigin.Lng, this.BaiduProjectedOrigin.Lat, this.ProjectedBounds.Left, this.ProjectedBounds.Top, this.GetLevelResolution(zoom), 0x100, 0x100);
        }

        //private static Point2D LonLat2Mercator(Point2D lonLat)
        //{
        //    double x = ((lonLat.X * 3.1415926535897931) * 6378137.0) / 180.0;
        //    double num2 = Math.Log(Math.Tan(((90.0 + lonLat.Y) * 3.1415926535897931) / 360.0)) / 0.017453292519943295;
        //    return new Point2D(x, ((num2 * 3.1415926535897931) * 6378137.0) / 180.0);
        //}

        private static PointLatLng LonLat2Mercator(double X, double Y)
        {
            double x = ((X * 3.1415926535897931) * 6378137.0) / 180.0;
            double num2 = Math.Log(Math.Tan(((90.0 + Y) * 3.1415926535897931) / 360.0)) / 0.017453292519943295;
            double y = ((num2 * 3.1415926535897931) * 6378137.0) / 180.0;
            return new PointLatLng(y, x);
        }

        private PointLatLng MercatorToLonLat(double x, double y)
        {
            double lng = (x / (3.1415926535897931 * this.Axis)) * 180.0;
            y = (y / (3.1415926535897931 * this.Axis)) * 180.0;
            return new PointLatLng(57.295779513082323 * ((2.0 * Math.Atan(Math.Exp((y * 3.1415926535897931) / 180.0))) - 1.5707963267948966), lng);
        }

        public static GSize GetTile(double originX, double originY, double x, double y, double resolution, int tileWidth = 0x100, int tileHeight = 0x100)
        {
            double d = (x - originX) / (tileWidth * resolution);
            double num2 = (originY - y) / (tileHeight * resolution);
            long width = (long)Math.Floor(d);
            return new GSize(width, (long)Math.Floor(num2));
        }

        // Properties
        public override double Axis
        {
            get
            {
                return 6378137.0;
            }
        }

        //private Point2D BaiduProjectedOrigin
        //{
        //    get
        //    {
        //        return new Point2D(-this.GetLevelResolution(1) * 256.0, this.GetLevelResolution(1) * 256.0);
        //    }
        //}

        private PointLatLng BaiduProjectedOrigin
        {
            get
            {
                return new PointLatLng(this.GetLevelResolution(1) * 256.0, -this.GetLevelResolution(1) * 256.0);
            }
        }

        public override RectLatLng Bounds
        {
            get
            {
                return RectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);
            }
        }

        public override double Flattening
        {
            get
            {
                return 0.0033528106647474627;
            }
        }

        //private BoundingBox ProjectedBounds
        //{
        //    get
        //    {
        //        return new BoundingBox(-3.1415926535897931 * this.Axis, -3.1415926535897931 * this.Axis, 3.1415926535897931 * this.Axis, 3.1415926535897931 * this.Axis, null);
        //    }
        //}

        private BoundingBox ProjectedBounds
        {
            get
            {
                return new BoundingBox(-3.1415926535897931 * this.Axis, -3.1415926535897931 * this.Axis, 3.1415926535897931 * this.Axis, 3.1415926535897931 * this.Axis);
            }
        }

        public PointLatLng TileOrigin
        {
            get
            {
                return this.MercatorToLonLat(this.BaiduProjectedOrigin.Lng, this.BaiduProjectedOrigin.Lat);
            }
        }

        public override GSize TileSize
        {
            get
            {
                return this.tileSize;
            }
        }

    }

    internal class BoundingBox
    {
        public double Left { set; get; }

        public double Bottom { set; get; }

        public double Right { set; get; }

        public double Top { set; get; }

        public BoundingBox(double left, double bottom, double right, double top)
        {
            Left = left;
            Bottom = bottom;
            Right = right;
            Top = top;
        }
    }
}
