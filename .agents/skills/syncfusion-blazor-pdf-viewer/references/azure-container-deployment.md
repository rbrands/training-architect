# Azure Container Deployment for Blazor PDF Viewer

> **Important Note**: This reference is for **information and guidance only**. Do NOT make any changes to project files, Dockerfiles, or Azure configurations based on this documentation unless explicitly requested by the user.

## Overview

This guide provides comprehensive instructions for containerizing and deploying Blazor PDF Viewer applications (both Server and WebAssembly scenarios) to Azure using Azure Container Registry (ACR) and Azure App Service for Containers.

---

## Prerequisites

Before proceeding with Azure container deployment:

- **Azure Subscription**: Active Azure subscription with permissions to create:
  - Resource groups
  - Azure Container Registry (ACR) instances
  - App Services
- **Docker**: Docker Desktop or Rancher Desktop installed locally
- **Blazor PDF Viewer Sample**: A working Blazor PDF Viewer application (refer to getting started guide)

---

## Deployment Scenarios

### 1. Docker Web App (Server and WebAssembly)

#### Dockerfile Configuration for .NET 10/.NET 9/.NET 8 (Server Mode)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# install System.Drawing native dependencies
RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus libc6-dev libx11-dev
RUN ln -s libgdiplus.so gdiplus.dll

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["YourServerApp/YourServerApp.csproj", "YourServerApp/"]
RUN dotnet restore "./YourServerApp/YourServerApp.csproj"
COPY . .
WORKDIR "/src/YourServerApp"
RUN dotnet build "./YourServerApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./YourServerApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YourServerApp.dll"]
```

**Key Points**:
- Replace `YourServerApp.dll` and `YourServerApp.csproj` with actual assembly name
- For WebAssembly, add: `RUN dotnet workload install wasm-tools`
- Native dependencies (`libgdiplus`, `libc6-dev`, `libx11-dev`) are required for PDF rendering

#### Local Build and Test Commands

```bash
# Build the Docker image
docker build -t pdfviewerwebservice:latest .

# Run the container locally
docker run -d -p 6002:80 pdfviewerwebservice:latest

# Access at http://localhost:6002
```

**Troubleshooting Note**: If documents fail to load or script errors occur, verify `libgdiplus` is properly installed in the container image.

---

### 2. Docker Standalone WebAssembly App

#### Project Structure Requirements

For standalone WebAssembly samples, add these files to the project:
- `Dockerfile`
- `nginx.conf`
- `NuGet.Config`
- Empty `package` folder

#### Dockerfile for Standalone WebAssembly

```dockerfile
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# install System.Drawing native dependencies

RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus libc6-dev libx11-dev

RUN ln -s libgdiplus.so gdiplus.dll
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
## Install Python required for WASM tools
RUN apt-get update && apt-get install -y \
        python3 \
        python3-pip \
        python3-venv \
    && ln -s /usr/bin/python3 /usr/bin/python || true
## Install WASM tools
RUN dotnet workload install wasm-tools
WORKDIR /src
COPY ["NuGet.Config","/src/"]
COPY ["package", "/src/package"]

RUN dotnet nuget add source package
COPY ["WasmStandalone.csproj", "."]
RUN apt-get update && apt-get install -y emscripten
RUN dotnet restore "WasmStandalone.csproj" --configfile "NuGet.Config"
COPY . .
RUN dotnet build "WasmStandalone.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WasmStandalone.csproj" -c Release -o /app/publish 

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
```

**Key Points**:
- Replace `WasmStandalone.csproj` with your actual project name
- `wasm-tools` workload installation is required: `RUN dotnet workload install wasm-tools`
- Final stage uses nginx for serving static WebAssembly files

#### Local Build and Test Commands for WebAssembly

```bash
# Build the Docker image
docker build -t webassembly .

# List Docker images
docker image ls

# Run the container
docker run -d -p 6003:80 pdfviewer-wasm:latest

# Access at http://localhost:6003
```

---

## Azure Container Registry (ACR) Setup

### Method 1: Azure Portal (UI-Driven)

#### Step 1: Create Container Registry

1. In Azure portal, click **Create a resource**
2. Search for **Container Registry** → **Create**
3. Fill in basic details:
   - **Registry name**: Unique name
   - **Subscription**: Select subscription
   - **Resource group**: Create new or use existing
   - **Location**: Choose region
   - **SKU**: Basic (sufficient for testing)
4. Click **Review + create**, then **Create**

#### Step 2: Enable Admin Credentials

1. Open the newly created Container Registry
2. Navigate to **Settings** → **Access keys**
3. Toggle **Admin user** to **Enabled**
4. Note the following credentials:
   - **Login server**
   - **Username**
   - **Password**

#### Step 3: Tag and Push Image

Open PowerShell or terminal on your build machine:

```powershell
# Log in to Azure Container Registry
docker login <login-server>
# Enter username and password from Access keys

# Tag the image for your registry
docker tag pdfviewerwebservice:latest <login-server>/pdfviewerwebservice:latest

# Push the image to ACR
docker push <login-server>/pdfviewerwebservice:latest
```

---

## Azure App Service Setup

### Create App Service for Container (Linux)

#### Step 1: Create the App Service

1. In Azure portal, click **Create a resource**
2. Search for **Web App** → **Create**
3. Under **Basics** tab:
   - **Subscription**: Select subscription
   - **Resource group**: Create new or use existing
   - **Name**: Choose app name (forms the URL)
4. **Publish**: Choose **Docker Container**
5. **Runtime stack**: Choose **Linux**
6. Choose hosting plan:
   - Click **Change size**
   - Select **Basic/B1 or higher** (recommended for container workloads)
7. Click **Review + create**, then **Create**

#### Step 2: Configure Container Settings

1. Open the created App Service
2. Navigate to **Deployment** → **Container settings** (or **Settings** → **Container settings**)
3. **Image source**: Select **Azure Container Registry**
4. Select your **Subscription** and **Registry** (created earlier)
5. Under **Image and tag**:
   - **Repository**: Select `pdf_viewer_web_service` (or your image name)
   - **Tag**: Select `latest`
6. Click **Save**

---

## Troubleshooting Guide

### Common Issues and Solutions

#### 1. App Fails to Start

**Possible Causes**:
- Check container logs in Azure App Service
- Verify image runs locally first
- Ensure container listens on correct port

**Solution**:
- Container must listen on port 80 by default
- Or configure App Service container port setting to match your container's port

#### 2. Document Rendering/Script Errors

**Possible Causes**:
- Missing native dependencies (SkiaSharp, `libgdiplus`)

**Solution**:
- Verify Dockerfile includes:
  ```dockerfile
  RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus libc6-dev libx11-dev
  RUN ln -s libgdiplus.so gdiplus.dll
  ```
- Test image locally to confirm rendering works

#### 3. WebAssembly WASM Loading Issues

**Possible Causes**:
- Incorrect MIME types for WASM files
- nginx configuration issues

**Solution**:
- Ensure `nginx.conf` includes proper WASM MIME type configuration
- Verify caching headers are set correctly

#### 4. Missing Dependencies

**Common Issue**: `libgdiplus installation issues`

**Solution**:
- Ensure `libgdiplus` is installed in the base image
- Verify symbolic link is created: `RUN ln -s libgdiplus.so gdiplus.dll`

---

## Additional Considerations

### Using Rancher Desktop / Docker on Windows

- **Rancher Desktop** can replace Docker Desktop
- Select the **docker** runtime for standard `docker` command compatibility
- For WSL or Rancher errors during installation, consult Rancher Desktop documentation

### SkiaSharp Requirements

- If using `SkiaSharp.Views.Blazor` on server or client:
  - Verify native runtime requirements
  - Test rendering in the container environment
  - Confirm all native dependencies are present

### Server Interactive Scenarios

- Register Syncfusion services in `Program.cs`
- Configure SignalR message size settings for large-file processing:
  ```csharp
  builder.Services.AddSignalR(o => { 
      o.MaximumReceiveMessageSize = 102400000; 
  });
  ```

### WebAssembly Interactive Render Modes

- Ensure `wasm-tools` workload is installed:
  ```bash
  dotnet workload install wasm-tools
  ```
- For specific .NET versions:
  ```bash
  dotnet workload install wasm-tools-net8  # For .NET 8.0
  dotnet workload install wasm-tools-net9  # For .NET 9.0
  dotnet workload install wasm-tools-net10 # For .NET 10.0
  ```

---

## Deployment Result

Once successfully deployed, your Blazor PDF Viewer application will be accessible via the Azure App Service URL.

---

## Key Deployment Commands Summary

### Docker Commands
```bash
# Build image
docker build -t pdfviewerwebservice:latest .

# Run locally
docker run -d -p 6002:80 pdfviewerwebservice:latest

# List images
docker image ls
```

### Azure Container Registry Commands
```powershell
# Login to ACR
docker login <login-server>

# Tag image
docker tag pdfviewerwebservice:latest <login-server>/pdfviewerwebservice:latest

# Push to ACR
docker push <login-server>/pdfviewerwebservice:latest
```

---

## Important Reminders

1. **Read-Only Reference**: This documentation is for guidance only
2. **No Automatic Changes**: Do not apply these changes to project files without explicit user permission
3. **User Confirmation Required**: Always confirm with the user before modifying:
   - Dockerfiles
   - `.csproj` files
   - `Program.cs`
   - Azure configurations
4. **Testing First**: Always recommend local testing before Azure deployment
5. **Security Considerations**: Remind users to secure ACR credentials and manage access keys properly

---
