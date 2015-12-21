=====================================
Unity Spline Creator / Extruder tool 
Nick Vanheer
Video: https://youtu.be/bwsZGlNbWBE 
=====================================

=============
How to use
=============

==== 1: SPLINE CREATION ====
In order to construct a spline, add the SplineCreator component to an empty GameObject.
Click the "Add spline" button to add a spline (this will add a Bezier GameObject, 
and make sure it's linked with the rest of the spline.).
You can move the control points of each of the spline's beziers around to get the desired shape

==== 2: PATH CREATION ====
If you want to create a path following this spline you can add the Bezier Shape Extruder component to the spline.
This component automatically extrudes a path alongside the spline, as long as it has a Spline Creator. 
You can set the width or upwards extrusion properties.
By default the path will update in real-time when you move the control points. 
(although this is coded quite dirty, feel free to update)

==== HANDY & INFO ====

You can drag a readymade prefab out of the Resources/Prefab folder called SplineExample. 
Press the Extrude button to extrude the mesh

You should always use the SplineCreator component, even if you want only one spline 
(i.e the bezier) as other classes depend on it.

