using HyperTracker.Core;
using HyperTracker.CV;

namespace TestCore;

public class CalibrationMeshTest
{


    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void TestSimpleMeshAddColumn()
    {
        CalibrationMesh _emptySimpleMesh = new CalibrationMesh(CalibrationType.SIMPLE, new List<List<Point>>());
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
    }

    [Test]
    public void TestSimpleMeshAddMultipleColumn()
    {
        CalibrationMesh _emptySimpleMesh = new CalibrationMesh(CalibrationType.SIMPLE, new List<List<Point>>());
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
    }

    [Test]
    public void TestSimpleMeshAddRow()
    {
        CalibrationMesh _emptySimpleMesh = new CalibrationMesh(CalibrationType.SIMPLE, new List<List<Point>>());
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
        Assert.True(_emptySimpleMesh.MeshPoints[0].Count == 2);
        _emptySimpleMesh.AddRow();
        Assert.True(_emptySimpleMesh.MeshPoints[0].Count == 2);
    }

    [Test]
    public void TestSimpleMeshRemoveRow()
    {
        CalibrationMesh _emptySimpleMesh = new CalibrationMesh(CalibrationType.SIMPLE, new List<List<Point>>());
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
        Assert.True(_emptySimpleMesh.MeshPoints[0].Count == 2);
        _emptySimpleMesh.RemoveRow(0);
        Assert.True(_emptySimpleMesh.MeshPoints[0].Count == 1);
    }

    [Test]
    public void TestSimpleMeshRemoveColumn()
    {
        CalibrationMesh _emptySimpleMesh = new CalibrationMesh(CalibrationType.SIMPLE, new List<List<Point>>());
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
        Assert.True(_emptySimpleMesh.MeshPoints[0].Count == 2);
        _emptySimpleMesh.RemoveColumn(0);
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 0);
    }

    [Test]
    public void TestSimpleMeshUpdatePoint()
    {
        CalibrationMesh _emptySimpleMesh = new CalibrationMesh(CalibrationType.SIMPLE, new List<List<Point>>());
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
        Assert.True(_emptySimpleMesh.MeshPoints[0].Count == 2);
        _emptySimpleMesh.UpdateMeshPoint(0, 0, new Point(50, 90));
        Assert.True(_emptySimpleMesh.MeshPoints[0][0].y == 90);
    }

    [Test]
    public void TestSimpleMeshShiftRow()
    {
        CalibrationMesh _emptySimpleMesh = new CalibrationMesh(CalibrationType.SIMPLE, new List<List<Point>>());
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
        Assert.True(_emptySimpleMesh.MeshPoints[0].Count == 2);
        _emptySimpleMesh.UpdateMeshPoint(0, 0, new Point(50, 90));
        _emptySimpleMesh.UpdateMeshPoint(1, 0, new Point(50, 80));
        _emptySimpleMesh.ShiftRow(0, 1);
        Assert.True(_emptySimpleMesh.MeshPoints[0][0].y == 91);
        _emptySimpleMesh.ShiftRow(0, 10);
        Assert.True(_emptySimpleMesh.MeshPoints[0][0].y == 91);
        _emptySimpleMesh.ShiftRow(1, 1);
        Assert.True(_emptySimpleMesh.MeshPoints[0][1].y == 81);
        _emptySimpleMesh.ShiftRow(1, 10);
        Assert.True(_emptySimpleMesh.MeshPoints[0][1].y == 81);
        _emptySimpleMesh.ShiftRow(1, -90);
        Assert.True(_emptySimpleMesh.MeshPoints[0][1].y == 81);


    }

    [Test]
    public void TestSimpleMeshMeasurement()
    {
        CalibrationMesh _emptySimpleMesh = new CalibrationMesh(CalibrationType.SIMPLE, new List<List<Point>>());
        _emptySimpleMesh.AddColumn();
        Assert.True(_emptySimpleMesh.MeshPoints.Count == 1);
        Assert.True(_emptySimpleMesh.MeshPoints[0].Count == 2);
        _emptySimpleMesh.UpdateMeshPoint(0, 0, new Point(50, 90));
        _emptySimpleMesh.UpdateMeshPoint(1, 0, new Point(50, 80));
        Assert.True(_emptySimpleMesh.MeasureDistance(new Point(0, 85), 10, 0) == 5);
        Assert.True(_emptySimpleMesh.MeasureDistance(new Point(0, 75), 10, 0) == 15);
        Assert.True(_emptySimpleMesh.MeasureDistance(new Point(0, 95), 10, 0) == -5);
        Assert.True(_emptySimpleMesh.MeasureDistance(new Point(0, 88), 5, 0) == 1);
    }

    
}