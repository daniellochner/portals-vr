![Thumbnail](https://user-images.githubusercontent.com/37551307/128268408-a3abd8c2-e236-43aa-bb27-8da8d0ba830e.png)

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](http://choosealicense.com/licenses/mit/)

PortalsVR includes functioning portals in Virtual Reality (VR) for Unity, and is adapted from [Portals](https://github.com/SebLague/Portals) (by Sebastian Lague) and [PocketPortalVR](https://github.com/andrewzimmer906/PocketPortalVR) (by Andrew Zimmer). It was made specifically for my research project on [VR Natural Walking in Impossible Spaces](https://daniellochner.itch.io/impossible-spaces-vr).

The following is an extract from my [research paper](https://doi.org/10.1145/3487983.3488305), which describe the main features:
> ### 1. Stereoscopic Rendering:
> The view through a portal should align with what the user would see if they were transformed to a position and rotation with respect to the destination portal equivalent to their position and rotation with respect to the source portal. In order to correctly portray the view through a portal in VR, different images of the subspace beyond the portal need to be projected as textures onto the portal surface for each eye. For this, portals each required an additional camera and screen. Screens were then assigned to separate layers, and the users’ eyes culled so as to only view one screen per eye. The left and right portal cameras. could then render separate views for each eye, while tracking the position and rotation of an alias for each eye. The additional camera and projected image per portal does impact performance. This is partially compensated for using frustum culling and occlusion to determine whether a portal needs rendering.
>
> ### 2. Teleportation:
> A pair of portals should allow users to travel instantaneously between them. In our case, portal pairs always have the same local position and rotation with respect to the physical room. Thus, the user’s rotation is not altered when teleporting, and so travel is achieved by simply offsetting the position of the player to the destination subspace.
>
> ### 3. Occlusion:
> Objects that fall in front of the portal surface (i.e., between the viewpoint and the portal) must not be rendered in the portal display. This is achieved by setting the camera’s near clipping plane to coincide with the portal surface.
> 
> ### 4. Clipping:
> Unfortunately, because the near clipping plane cannot be set to zero, a portal may be clipped away when the viewpoint approaches too closely. To overcome this, the portal surface is dynamically deformed to mould around the player’s head. This, however, allows objects on the other side of a portal to clip through. A solution is to include an array of clipped objects, which deactivate when behind the player. Another clipping issue occurs when the player’s avatar crosses the portal, specifically when the stereoscopic camera rig is on one side of a portal and models of the controllers or the clipboard are on the other. Because this is only a single player proof of concept, this can be circumvented by introducing an overlay camera, which ensures that these models are always rendered in front of other views.
