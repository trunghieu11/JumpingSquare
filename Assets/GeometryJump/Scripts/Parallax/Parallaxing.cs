﻿/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Parallax scrolling script that should be assigned to a layer
/// </summary>
namespace AppAdvisory.GeometryJump
{
	public class Parallaxing : MonoBehaviorHelper
	{
		/// <summary>
		/// Scrolling speed
		/// </summary>
		public Vector2 speed = new Vector2(10, 10);

		/// <summary>
		/// Moving direction
		/// </summary>
		public Vector2 direction = new Vector2(-1, 0);


		/// <summary>
		/// 1 - Background is infinite
		/// </summary>
		public bool isLooping = false;

		/// <summary>
		/// 2 - List of children with a renderer.
		/// </summary>
		private List<Transform> backgroundPart;

		private Vector3 previousCamPos;		//the position of the camera in the previous frame

		// 3 - Get all the children
		void Start()
		{
			previousCamPos = cam.transform.position;

			// For infinite background only
			if (isLooping)
			{
				// Get all the children of the layer with a renderer
				backgroundPart = new List<Transform>();

				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);

					// Add only the visible children
					if (child.GetComponent<Renderer>() != null)
					{
						backgroundPart.Add(child);
					}
				}

				// Sort by position.
				// Note: Get the children from left to right.
				// We would need to add a few conditions to handle
				// all the possible scrolling directions.
				backgroundPart = backgroundPart.OrderBy(
					t => t.position.x
				).ToList();
			}
		}
		public float smoothing = 1f;	//how smooth the parallax is going to be, Must be above 0 otherwize the parallax will not work
		void Update()
		{

			//the parallax is the opposite of the camera movement because the previous frame multiplied by the scale
			float parallax =  (previousCamPos.x - cam.transform.position.x) * 100;

			//set a target x position that is the current position plus the parallax
			float backgroundTargetPosX = transform.position.x + parallax;

			Vector3 backgroundTargetPos = new Vector3(backgroundTargetPosX, transform.position.y, transform.position.z);

			transform.position = Vector3.Lerp(transform.position, backgroundTargetPos, smoothing * Time.deltaTime);

			// 4 - Loop
			if (isLooping)
			{
				// Get the first object.
				// The list is ordered from left (x position) to right.
				Transform firstChild = backgroundPart.FirstOrDefault();

				if (firstChild != null)
				{
					// Check if the child is already (partly) before the camera.
					// We test the position first because the IsVisibleFrom
					// method is a bit heavier to execute.
					if (firstChild.position.x < cam.transform.position.x)
					{
						// If the child is already on the left of the camera,
						// we test if it's completely outside and needs to be
						// recycled.
						if (firstChild.GetComponent<Renderer>().IsVisibleFrom(Camera.main) == false)
						{

							float height = 2f * cam.orthographicSize;

							// Get the last child position.
							Transform lastChild = backgroundPart.LastOrDefault();
							Vector3 lastPosition = lastChild.transform.position;
							Vector3 lastSize = (lastChild.GetComponent<Renderer>().bounds.max - lastChild.GetComponent<Renderer>().bounds.min);

							// Set the position of the recyled one to be AFTER
							// the last child.
							// Note: Only work for horizontal scrolling currently.
							//						firstChild.position = new Vector3(lastPosition.x + lastSize.x, firstChild.position.y, firstChild.position.z);
							firstChild.position = new Vector3(lastPosition.x + lastSize.x, cam.transform.position.y - height/2f, firstChild.position.z);

							// Set the recycled child to the last position
							// of the backgroundPart list.
							backgroundPart.Remove(firstChild);
							backgroundPart.Add(firstChild);
						}
					}
				}
			}

			//set the previousCamPos to the camera's position at the end of the frame
			previousCamPos = cam.transform.position;
		}
	}
}