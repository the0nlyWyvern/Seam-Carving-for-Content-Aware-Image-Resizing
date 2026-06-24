# Seam Carving for Content-Aware Image Resizing

<img src="./resources/cover.jpg">

## Description

Seam Carving is a content-aware image resizing technique that preserves important image features by removing or inserting low-energy pixel seams. This implementation supports image reduction and expansion with less distortion than traditional scaling approaches.

This project demonstrates several fundamental concepts in image processing and algorithm design. It includes convolution using kernels, gradient or magnitude computation for vector fields, and dynamic programming for seam selection. Through the implementation of seam carving, readers can gain practical experience with image filtering, vector-based analysis, path optimization, and content-aware image manipulation.

## Requirement

- .NET SDK
- SixLabors.ImageSharp 3.1.11

## Setup

1. Clone the repository
2. Restore NuGet packages: `dotnet restore`
3. Build the project: `dotnet build`
4. Run the application: `dotnet run`

<sub>Important Note: This project is currently configured to use SixLabors.ImageSharp 3.1.11. Newer versions of ImageSharp may require additional licensing configuration and could cause build errors. If restoring packages manually, ensure that version 3.1.11 is installed.</sub>

## Process

The below example describes the process of seam carving to shrink 30% of the original width while preserving height (`.ShrinkTo("100%", "70%")`).

| Step                                                                                          | Image                                                                      |
| --------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| 0. Start with Broadway tower image.                                                           | <img src="./InputImages/BroadwayTower.jpg" height="170">                   |
| 1. Convert from colored image to grayscale image.                                             | <img src="./resources/process/Grayscale_BroadwayTower.jpg" height="170">   |
| 2. Calculate the energy of each pixel or create an edge map <br>using edge detection kernels. | <img src="./resources/process/Edgemap_BroadwayTower.jpg" height="170">     |
| 3. Find the lowest energy seam.                                                               |                                                                            |
| 4. Remove seam and recalculate edge/energy map.                                               |                                                                            |
| 5. Repeat steps 3 and 4 until reaching desired width.                                         |                                                                            |
| All lowest energy seams are marked as red.                                                    | <img src="./resources/process/MarkedSeams_BroadwayTower.jpg" height="170"> |
| Final image.                                                                                  | <img src="./resources/process/Carved_BroadwayTower.jpg" height="170">      |

<br><br>
The below example describes the process of seam carving of 150%-width expansion (`ExpandWidthTo("150%")`) and 200%-width expansion(`.ExpandWidthTo("200%")`)

| Step                                                                                                                   | Image                                                                                    |
| ---------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| 0. Start with Broadway tower image.                                                                                    | <img src="./InputImages/BroadwayTower.jpg" height="170">                                 |
| 1. Perform seam carving technique <br>above to mark all the seam, same <br>as shrinking 50% width.                     | <img src="./resources/process/MarkedSeams_Expansion_BroadwayTower.jpg" height="170">     |
| 2. Add low-energy seams by <br>duplicating marked pixels.                                                              | <img src="./resources/process/Extended_BroadwayTower.jpg" height="170">                  |
| Expand to 200% width in one go will <br>add both low and high-energy seams, <br>stretching the objects and background. |                                                                                          |
| 3. Instead, perform a second expansion <br>on the 150%-width image to find lowest seams.                               | <img src="./resources/process/MarkedSeams_BroadwayTower_2ndExtension_.jpg" height="170"> |
| Final image.                                                                                                           | <img src="./resources/process/Extended_BroadwayTower_2ndExtension_.jpg" height="170">    |

## Other examples

#### The Persistence Of Memory (Salvador Dalí)

<table align="center" style="border: none; border-collapse: collapse;">
  <tr style="border: none;">
    <td align="center" style="border: none; padding: 10px;">
      <img src="./InputImages/ThePersistenceOfMemory.jpg" height="340"><br>
      <p>Original image</p>
    </td>
    <td align="center" style="border: none; padding: 10px;">
      <img src="./resources/examples/example0.jpg" height="340"><br>
      <p>50% width</p>
    </td>
  </tr>
</table>

<p align="center">
    <img src="./resources/examples/Expanded_ThePersistenceOfMemory.jpg" height="340"> <br> 
    150% width
</p>

<p align="center">
    <img src="./resources/examples/Expanded_ThePersistenceOfMemory_2ndExtension_.jpg" height="340"> <br> 
    200% width
</p>

<br>
<br>

#### Broadway Tower

<table align="center" style="border: none; border-collapse: collapse;">
  <tr style="border: none;">
    <td align="center" style="border: none; padding: 10px;">
      <img src="./InputImages/BroadwayTower.jpg" height="450"> <br> 
      <p>Original image</p>
    </td>
    <td align="center" style="border: none; padding: 10px;">
      <img src="./resources/examples/Carved_BroadwayTower.jpg" height="360"><br>
      <p>80% height + 35% width</p>
    </td>
  </tr>
</table>

<br>
<br>

#### School Of Fish (Jeremy Bishop)

<p align="center">
    <img src="./InputImages/SchoolOfFish_JeremyBishop.jpg" height="500"> <br>
    Original image
</p>

<table align="center" style="border: none; border-collapse: collapse;">
  <tr style="border: none;">
    <td align="center" style="border: none; padding: 10px;">
      <img src="./resources/examples/Carved_SchoolOfFish_JeremyBishop.jpg" height="500"> <br> 
      <p>48% width</p>
    </td>
    <td align="center" style="border: none; padding: 10px;">
      <img src="./resources/examples/Carved_SchoolOfFish_JeremyBishop2.jpg" height="300"><br>
      <p>60% height + 50% width</p>
    </td>
  </tr>
</table>

<p align="center">
    <img src="./resources/examples/Expanded_SchoolOfFish_JeremyBishop.jpg" height="500"> <br> 
    150% width
</p>

<br>
<br>

#### Silhouette Of Trees (Dave Hoefler)

<table align="center" style="border: none; border-collapse: collapse;">
  <tr style="border: none;">
    <td align="center" style="border: none; padding: 10px;">
      <img src="./InputImages/SilhouetteOfTrees_DaveHoefler.jpg" height="600"> <br> 
      <p>Original image</p>
    </td>
    <td align="center" style="border: none; padding: 10px;">
      <img src="./resources/examples/Carved_Sunset_DaveHoefler.jpg" height="600"><br>
      <p>70% width</p>
    </td>
  </tr>
</table>

<p align="center">
    <img src="./resources/examples/Expanded_Sunset_DaveHoefler_2ndExtension_.jpg" height="600"> <br> 
    200% width
</p>

#### Snow Fox On Snow Field (Jonatan Pie)

<table align="center" style="border: none; border-collapse: collapse;">
  <tr style="border: none;">
    <td align="center" style="border: none; padding: 10px;">
      <img src="./InputImages/SnowFoxOnSnowField_JonatanPie.jpg" height="300"> <br>
      <p>Original image</p>
    </td>
    <td align="center" style="border: none; padding: 10px;">
      <img src="./resources/examples/Carved_SnowFoxOnSnowField_JonatanPie.jpg" height="300"><br>
      <p>40% width</p>
    </td>
  </tr>
</table>

<p align="center">
    <img src="./resources/examples/Expanded_SnowFoxOnSnowField_JonatanPie_2ndExtension_.jpg" height="300"> <br> 
    200% width
</p>

#### Lake And Trees (Alice Triquet)

<table align="center" style="border: none; border-collapse: collapse;">
  <tr style="border: none;">
    <td align="center" style="border: none; padding: 10px;">
      <img src="./InputImages/LakeAndTrees_AliceTriquet.jpg" height="750"> <br>
      <p>Original image</p>
    </td>
    <td align="center" style="border: none; padding: 10px;">
      <img src="./resources/examples/Carved_LakeAndTrees_AliceTriquet.jpg" height="750"><br>
      <p>40% width</p>
    </td>
  </tr>
</table>

<p align="center">
    <img src="./resources/examples/Expanded_LakeAndTrees_AliceTriquet.jpg" height="750"> <br> 
    150% width
</p>

#### Herd Of Cows (Yang)

<table align="center" style="border: none; border-collapse: collapse;">
  <tr style="border: none;">
    <td align="center" style="border: none; padding: 10px;">
      <img src="./InputImages/HerdOfCows_Yang.jpg" height="750"> <br>
      <p>Original image</p>
    </td>
    <td align="center" style="border: none; padding: 10px;">
      <img src="./resources/examples/Carved_HerdOfCows_Yang.jpg" height="750"><br>
      <p>40% width</p>
    </td>
  </tr>
</table>

<p align="center">
    <img src="./resources/examples/Expanded_HerdOfCows_Yang.jpg" height="750"> <br> 
    150% width
</p>

## Contributors

Kiet Tran

## DIY

Want to create your own application? Check out these useful resources:

[But what is a convolution?](https://www.youtube.com/watch?v=KuXjwB4LzSA "youtube") - 3Blue1Brown - The beautiful of math through convolution.

Edge handling - [Extend](<https://en.wikipedia.org/wiki/Kernel_(image_processing)#:~:text=handling%20image%20edges.-,Extend,extended%20in%2090%C2%B0%20wedges.%20Other%20edge%20pixels%20are%20extended%20in%20lines.,-Wrap> "wikipedia") - During convolution process, you need to pick your strategy for handling image edges.

[Seam Carving | Week 2, lecture 7 | 18.S191 MIT Fall 2020](https://www.youtube.com/watch?v=rpB6zQNsbQU "youtube") - A detail explanation with visual examples on seam carving

# References

Avidan, Shai; Shamir, Ariel (July 2007). "Seam carving for content-aware image resizing". ACM SIGGRAPH 2007 papers. p. 10. doi:[10.1145/1275808.1276390](https://dl.acm.org/doi/10.1145/1275808.1276390 "ACM Digital Library"). ISBN 978-1-4503-7836-9.
