![Thumbnail](https://user-images.githubusercontent.com/37551307/128266997-32c2e585-3053-41a8-bcd6-8448386fc3c1.png)

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](http://choosealicense.com/licenses/mit/)

PortalsVR includes functioning portals in Virtual Reality (VR) for Unity, and is adapted from [Portals](https://github.com/SebLague/Portals) (by Sebastian Lague) and [PocketPortalVR](https://github.com/andrewzimmer906/PocketPortalVR) (by Andrew Zimmer). It was made specifically for my research project on "VR Natural Walking in Impossible Spaces".

The following is an extract from my research paper, which describe the main features:
> ### 1. Rendering:
> Portals’ views should align with what the user would be able to see if they were transformed to a position and rotation with respect to the linked portal, equivalent to
their position and rotation with respect to the viewed portal. Because Lague’s implementation catered to 3D, it did not allow for stereoscopic VR, which uses two images – one for each eye. For this, portals each needed an additional camera and screen. Screens were then assigned to separate layers, and the users’ eyes culled so as to only view one screen per eye. The left and right portal cameras could then render separate views for each eye, while tracking the position and rotation of an alias for each eye. Because of the fact that twice as many cameras were now being used, several necessary measures were included to ensure adequate performance, such as using the player camera’s view frustum to determine whether to render a portal.
>
> ### 2. Teleportation:
> A pair of portals should allow users to travel between them. In our case, pairs of portals always have the same local position and rotation with respect to the room they are in. This implies that the user’s rotation is not altered when teleporting, and so travel is achieved by simply offsetting the position of the player.
>
> ### 3. Clipping:
> Portals should prevent a user’s eyes from clipping through the screen when they are too close. This occurs because cameras are required to have a near clipping plane ≥ 0.01. To overcome this, screens are dynamically deformed to mould around the player’s head. This, however, allows objects on the other side of a portal’s screen to clip through. A solution is to include an array of ‘clipped objects’, which deactivate when on the opposite side of the player. Another instance of clipping occurs when the models of controllers and the clipboard are moved through a portal’s screen. Because this is only a single player demonstration, this can be avoided by introducing an overlay camera, which ensures that these models are always rendered above everything else.
>
> ### 4. Occlusion:
> Portals should occlude objects in between the portal’s screen and its camera. This is achieved by adjusting the camera’s near clipping plane to fit the portal’s screen.
