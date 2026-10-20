# ASP.NET MVC DOCX Editor – Side by Side Comparison of Original and Revised Documents

## Introduction

This repository contains an ASP.NET MVC application that demonstrates how to compare two Word documents side by side using the Syncfusion<sup style="font-size:70%">&reg;</sup> [ASP.NET MVC DOCX Editor](https://www.syncfusion.com/docx-editor-sdk/asp-net-mvc-docx-editor?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples) (Document Editor). The sample allows users to upload an original and a revised document, compare them using the Document Editor web service, and view the comparison results with revision tracking.

### Features

- Upload an original Word document.
- Upload a revised Word document.
- Compare both documents.
- View tracked revisions in the comparison result.
- Switch between:
  - Original vs Compared document
  - Original vs Revised document
- Download the comparison result as a DOCX document with tracked changes preserved.

---

# How to Run the Application

## Prerequisites

Before running the sample, ensure the following software is installed:

- Visual Studio 2019 or later (or Visual Studio 2022)
- .NET Framework 4.7.2 SDK
- Internet access to restore NuGet packages

---

## Configuration

No additional configuration is required.

This sample is already configured to use the online Syncfusion Document Editor service:

```ts
var serviceUrl = 'https://document.syncfusion.com/web-services/docx-editor/api/documenteditor/'; //only for demo purpose.
```

Since the application consumes the hosted web service, you do **not** need to download, build, or run any ASP.NET Core Web API project.

> **Note:** The online web service is used for demo purposes only. For production scenarios, host your own web service using the [Syncfusion Document Editor Web API](https://help.syncfusion.com/document-processing/word/word-processor/asp-net-mvc/web-service).

---

## Restore the NuGet Packages

Open the `DocumentEditor.sln` file in the sample folder using Visual Studio. Visual Studio will automatically restore the NuGet packages on build. If not, right-click the solution in Solution Explorer and select **Restore NuGet Packages**.

---

## Run the Application

Press **F5** or click **Run** in Visual Studio. The application will launch in your default browser.

---

## Verify the Sample

1. Launch the application. The sample is configured with the Syncfusion **Tailwind** theme (`tailwind.css`).
2. Click **Choose file** under **Original document** and select a DOCX file.
3. Click **Choose file** under **Revised document** and select another version of the same document.
4. Click **Compare**.
5. The application sends both documents to the online Document Editor service.
6. The comparison result is displayed in the right-side editor with tracked revisions.
7. Toggle the **Show comparison results with tracked changes** checkbox to switch between:
   - **Original vs Compared**
   - **Original vs Revised**
8. Review insertions, deletions, and formatting changes using the Document Editor revision features.
9. Click **Download Result Document** to save the compared document with the tracked changes.

---
## Demo

<p align="center">
  <img
    src="images/compare-documents-demo-react-docx-editor.gif"
    alt="ASP.NET MVC DOCX Editor Compare Documents Demo"
    width="1000" />
</p>

---
# Project Workflow

The application follows the workflow below:

1. User uploads the original document.
2. User uploads the revised document.
3. Both documents are sent to the Syncfusion online Document Editor service.
4. The service:
   - Imports the DOCX files.
   - Compares both documents.
   - Generates a document with tracked revisions.
5. The comparison result is returned in SFDT format.
6. The ASP.NET MVC application loads the original document in the left editor.
7. The compared document (or revised document) is loaded in the right editor for side-by-side review.

```
Original DOCX
        │
        │
        ▼
ASP.NET MVC Application
        │
        │
        ▼
Syncfusion Online Document Editor Service
        │
        ├── Import Original
        ├── Import Revised
        └── Compare Documents
                │
                ▼
      Compared SFDT Document
                │
                ▼
ASP.NET MVC DOCX Editor
        │
        ├── Left Editor  → Original
        └── Right Editor → Compared / Revised
```

---

# Resources

- **Product page:**   [Syncfusion® ASP.NET MVC DOCX Editor](https://www.syncfusion.com/docx-editor-sdk/asp-net-mvc-docx-editor?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples) 

- **Documentation:**   [Syncfusion® ASP.NET MVC DOCX Editor - Documentation](https://help.syncfusion.com/document-processing/word/word-processor/asp-net-mvc/overview?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples) 

- **Online demo:**   [Syncfusion® ASP.NET MVC DOCX Editor - Online demo](https://document.syncfusion.com/demos/docx-editor/asp-net-mvc/documenteditor/defaultfunctionalities?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples#/tailwind3)

# Support and feedback 

For any other queries, reach our [Syncfusion® support team](https://support.syncfusion.com/?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples) or post the queries through the [community forums](https://www.syncfusion.com/forums?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples). 

# License 

This is a commercial product and requires a paid license for possession or use. Syncfusion's licensed software, including this component, is subject to the terms and conditions of [Syncfusion's EULA](https://www.syncfusion.com/license/studio/22.2.5/syncfusion_essential_studio_eula.pdf?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples). You can purchase a license [here](https://www.syncfusion.com/sales/products?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples) or start a free 30\-day trial [here](https://www.syncfusion.com/account/manage-trials/start-trials?utm_source=github&utm_medium=listing&utm_campaign=github-asp-net-mvc-docx-editor-examples).