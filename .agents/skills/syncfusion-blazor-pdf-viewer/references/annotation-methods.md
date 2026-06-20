# Syncfusion Blazor PDF Viewer - Annotation Methods

## Table of Contents
- [Overview](#overview)
- [Methods List](#methods-list)

## Overview

The Syncfusion Blazor PDF Viewer offers powerful APIs to programmatically control annotation interactions within a PDF document. Using these APIs, you can enable or modify annotation behavior without direct user toolbar interaction, making them especially useful for custom UI workflows, automated annotation scenarios, and advanced PDF interaction logic.

## Methods List
The following APIs provide access to annotation-related methods available in the Syncfusion Blazor PDF Viewer.

### SetAnnotationModeAsync
Set annotation type to be added in next user interaction of the loaded PDF Document in the PDF Viewer. We can customize the annotation based on the user need. It will set annotation interaction thorugh the method.

#### Parameters
- [AnnotationType](./annotation-general-enum-properties.md#annotationtype)

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.SetAnnotationModeAsync(AnnotationType.Highlight);
```

### SetAnnotationModeAsync (Only for stamp annotation)
Switch on the annotation mode for stamp annotations. By the methods overloading of the `SetAnnotationModeAsync`, we can able to set the annotation type that we need to add the types(DynamicStampItem, SignStampItem, StandardBusinessStampItem) | `Task` | When we need annotation interatcion for stamp annotation programmatically |

#### Parameters
- [AnnotationType](./annotation-general-enum-properties.md#annotationtype)
- [DynamicStampItem?](./annotation-enum-properties.md#dynamicstampitem)
- [SignStampItem?](./annotation-enum-properties.md#signstampitem)
- [StandardBusinessStampItem?](./annotation-enum-properties.md#standardbusinessstampitem)

**Note** - ? is means to be optional paramter.

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.

    // We need to add dynamic stamp annotation, set the second paramter alone as needed type.
    await viewer.SetAnnotationModeAsync(AnnotationType.Stamp, DynamicStampItem.Approved);

    // We need to add sign here stamp annotation, set the second parameter as null and thrid parameter with needed type.
    await viewer.SetAnnotationModeAsync(AnnotationType.Stamp, null, SignStampItem.Accepted);

    // We need to add standard business stamp annotation, set the second and third parameter as null and fourth parameter with needed type.
    await viewer.SetAnnotationModeAsync(AnnotationType.Stamp, null, null, StandardBusinessStampItem.Approved);
```

### UpdateMeasurementSettingsAsync
Update measure settings for existing measure annotations | - | `Task` | When we want to update the measure annotation settings | Programmatically |

#### Parameters
No Paramters

#### Return Type
- `Task`

#### Code Example
```csharp
    // We can bind the method for click action based the user requirement.
    await viewer.UpdateMeasurementSettingsAsync();
```