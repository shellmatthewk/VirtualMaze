using System.Collections.Specialized;
using System.Numerics;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

namespace VirtualMaze.Assets.Scripts.Raycasting
{
   
    public class RelativeHitLocFinder {
        


        public static Vector2 getRelativeHit(RaycastHit raycastHit){
            Vector3 normal = raycastHit.normal;
            
            if (Math.Abs(normal.x) <= 0.0001 || Math.Abs(normal.z) <= 0.0001) {
                return getRelativeHitForVertical(raycastHit);
            }
            
            GameObject gameObjectHit = raycastHit.transform.gameObject;
            Vector3 hitLoc = raycastHit.point;
            // calculate relative on surface
            GameObject objectMaster = getMaster(gameObjectHit);
            Vector3 relative3D = objectMaster.transform.InverseTransformPoint(hitLoc);
            Vector3 projection = Vector3.Dot(relative3D,normal)  * normal;
            // normal is a unit vector, and this is a standard formula (https://en.wikipedia.org/wiki/Vector_projection)
            Vector3 rejection = relative3D - projection;
            // rejection is the sum of x and y relatives (in 2d form), it's the "hypotenuse"

            // cross of (normal,y) gives relative x-vector
            Vector3 relativeXUnitVector = Vector3.Cross(Vector3.up, normal);
            float relativeX = Vector3.Dot(rejection, relativeXUnitVector);
            float relativeY = (float)Math.Sqrt(rejection.sqrMagnitude - Math.Pow(relativeX, 2));

            return new Vector2(relativeX, relativeY);
            
        }


        private static Vector2 getRelativeHitForVertical(RaycastHit raycastHit) {
            // figure thit out
            // we are assured that it's a vertical surface
            // On ground, +x then +z
            // On ceiling, same
            Vector3 relativePoint = getMaster(raycastHit.transform.gameObject).transform.InverseTransformPoint(raycastHit.point);
            return new Vector2(relativePoint.x, relativePoint.z);
        }
        


        public static String getChainedName(GameObject gameObject) {

            String chainedName = gameObject.name;
            GameObject curObject = gameObject;
            while (curObject.transform.parent.gameObject != null && (gameObject.CompareTag("ObjectMaster") != true)) {
                // implicitly, if the parent is null, we've hit the top of the tree and must return
                Poster posterScript; //Poster is a script, that is attached to poster objects.
                if  (gameObject.TryGetComponent<Poster>(out posterScript)) {
                    curObject = posterScript.AttachedTo;
                } else {
                    curObject = gameObject.transform.parent.gameObject; //wow
                }
                chainedName += "_" + curObject.name;
            }
            return chainedName;
        }

        private static GameObject getMaster(GameObject gameObject) {
            GameObject curObject = gameObject;
            while (curObject.transform.parent.gameObject != null && (gameObject.CompareTag("ObjectMaster") != true)) {
                // implicitly, if the parent is null, we've hit the top of the tree and must return
                Poster posterScript; //Poster is a script, that is attached to poster objects.
                if  (gameObject.TryGetComponent<Poster>(out posterScript)) {
                    curObject = posterScript.AttachedTo;
                } else {
                    curObject = gameObject.transform.parent.gameObject; //wow
                }

            }
            return curObject;
        }

    }

}