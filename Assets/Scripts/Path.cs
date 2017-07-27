using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : MonoBehaviour
{
    public Color color;
    List<Vector3> pathPoints;
    public List<Vector3> crossPathPoints;

    public float reachDist = 10.0f;
    public float halfSegmentSize = 6f;

    // Use this for initialization 

    void OnDrawGizmos()
    {
        InitListPoints();
        InitVectorCheckPoints();

        Vector3 cubeSize = new Vector3(halfSegmentSize * 2, halfSegmentSize * 2, halfSegmentSize * 2);
        Vector3 start, end;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
            Gizmos.DrawWireCube(pathPoints[i], cubeSize);
            Gizmos.color = Color.blue;
            start = pathPoints[i] + crossPathPoints[i] * halfSegmentSize;
            end = pathPoints[i] - crossPathPoints[i] * halfSegmentSize;
            Gizmos.DrawLine(start, end);
            //Gizmos.DrawSphere(start, 1f);
            //Gizmos.DrawSphere(end, 1f);
        }
        Gizmos.DrawWireCube(pathPoints[pathPoints.Count - 1], cubeSize);
        Gizmos.DrawLine(pathPoints[pathPoints.Count - 1], pathPoints[0]);
        start = pathPoints[pathPoints.Count - 1] + crossPathPoints[pathPoints.Count - 1] * halfSegmentSize;
        end = pathPoints[pathPoints.Count - 1] - crossPathPoints[pathPoints.Count - 1] * halfSegmentSize;
        Gizmos.DrawLine(start, end);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pathPoints[0], cubeSize);
    }



    void InitListPoints()
    {
        pathPoints = new List<Vector3>();
        Transform[] transformList = GetComponentsInChildren<Transform>();

        foreach (Transform t in transformList)
        {
            if (t != transform)
                pathPoints.Add(t.position);
        }
    }

    void InitVectorCheckPoints()
    {
        crossPathPoints = new List<Vector3>();
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 a = (pathPoints[PrevCheckPoint(i)] - pathPoints[i]).normalized;
            a.y = 0;
            Vector3 b = (pathPoints[NextCheckPoint(i)] - pathPoints[i]).normalized;
            b.y = 0;
            crossPathPoints.Add((a + b).normalized);
        }
    }

    /*float poly_bezier(float[] B, float t) 
    { 
        float t_ = 1 - t; 
        return (t_ * t_ * t_ * B[0]) + (3 * t_ * t_ * t * B[1]) + (3 * t_ * t * t * B[2]) + (t * t * t * B[3]); 
    } 
 
    void dessiner_bezier(float[] Bx, float[] By, float[] Bz, float pas) 
    { 
        int i; 
        float x_prec = poly_bezier(Bx, 0),  
            y_prec = poly_bezier(By, 0), 
            z_prec = poly_bezier(Bz, 0), 
            x_curr = 0, y_curr = 0, z_curr = 0; 
        for (i = 1; i * pas < 1; i++) 
        { 
            x_curr = poly_bezier(Bx, i * pas); 
            y_curr = poly_bezier(By, i * pas); 
            z_curr = poly_bezier(Bz, i * pas); 
            //gdk_draw_line(win, gc, x_prec, y_prec, x_curr, y_curr); 
            Gizmos.DrawLine(new Vector3(x_prec, y_prec, z_prec), new Vector3(x_curr, y_curr, z_curr)); 
            x_prec = x_curr; 
            y_prec = y_curr; 
            z_prec = z_curr; 
        } 
        //gdk_draw_line(win, gc, x_prec, y_prec, Bx[3], By[3]); 
        Gizmos.DrawLine(new Vector3(x_prec, y_prec, z_prec), new Vector3(Bx[3], By[3], Bz[3])); 
    } 
 
    void convertir_cords(float[] P, int i, float[] B) 
    { 
        int j; 
        for (j = 0; j < 4; j++) 
        { 
            switch (j) 
            { 
                case 0: B[0] = (P[i] + 4 * P[i + 1] + P[i + 2]) / 6; break; 
                case 1: B[1] = (2 * P[i + 1] + P[i + 2]) / 3; break; 
                case 2: B[2] = (P[i + 1] + 2 * P[i + 2]) / 3; break; 
                case 3: B[3] = (P[i + 1] + 4 * P[i + 2] + P[i + 3]) / 6; break; 
                default: break; 
            } 
        } 
    } 
 
    void dessiner_bspline(float pas) 
    { 
        int i, j; 
        float[] Bx = new float[4]; 
        float[] By = new float[4]; 
        float[] Bz = new float[4]; 
        int Pn = pathPoints.Count; 
        float[] Px = new float[Pn]; 
        float[] Py = new float[Pn]; 
        float[] Pz = new float[Pn]; 
 
        for (i = 0; i < Pn; i++) 
        { 
            Px[i] = pathPoints[i].position.x; 
            Py[i] = pathPoints[i].position.y; 
            Pz[i] = pathPoints[i].position.z; 
        } 
 
        for (i = 0; i < Pn - 3; i++) 
        { 
            convertir_cords(Px, i, Bx); 
            convertir_cords(Py, i, By); 
            convertir_cords(Pz, i, Bz); 
            //set_color(gc, 100, 100, 100); 
            Gizmos.color = new Color(100, 100, 100); 
            //gdk_draw_line(win, gc, Bx[0], By[0], Bx[1], By[1]); 
            //gdk_draw_line(win, gc, Bx[2], By[2], Bx[3], By[3]); 
            Gizmos.DrawLine(new Vector3(Bx[0], By[0], Bz[0]), new Vector3(Bx[1], By[1], Bz[1])); 
            Gizmos.DrawLine(new Vector3(Bx[2], By[2], Bz[2]), new Vector3(Bx[3], By[3], Bz[3])); 
            /*for (j = 0; j < 4; j++) 
            { 
                gdk_draw_arc(win, gc, FALSE, Bx[j] - 3, By[j] - 3, 6, 6, 0, 360 * 64); 
            } 
            //set_color(gc, 200, 50, 200); 
            Gizmos.color = new Color(200, 50, 200); 
            dessiner_bezier(Bx, By, Bz, pas); 
        } 
 
    }*/

    int NextCheckPoint(int point)
    {
        return (point + 1) % pathPoints.Count;
    }

    int PrevCheckPoint(int point)
    {
        if (point == 0)
            return pathPoints.Count - 1;
        return (point - 1);
    }

}
