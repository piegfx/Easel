﻿#!/bin/bash

# WARNING! Requires that you have the Vulkan toolchain and glslc installed.

printf "Compiling HLSL shaders...\n"
for file in $(find . -type f -name "*.hlsl"); do
  printf "Compiling \"%s\"... " "$file"
  
  filename=${file%.*}
  
  hasCompiled=false 
  
  if grep -q "VertexShader" "$file"; then
    glslc -fshader-stage=vertex -fentry-point="VertexShader" -fauto-combined-image-sampler -o "${filename}_vert.spv" "$file"
    hasCompiled=true
  fi
  
  if grep -q "PixelShader" "$file"; then
    glslc -fshader-stage=fragment -fentry-point="PixelShader" -fauto-combined-image-sampler -o "${filename}_frag.spv" "$file"
    hasCompiled=true
  fi
  
  if [ $? -ne 0 ]; then
    exit 1
  fi
  
  if ! $hasCompiled; then
    printf "Ignoring as this file doesn't contain an entry point.\n"
  else
    printf "Done!\n"
  fi
done

printf "Compiling GLSL shaders...\n"
for file in $(find . -type f -name "*.vert"); do
  printf "Compiling \"%s\"... " "$file"
  
  filename=${file%.*}
  
  glslc -o "${filename}_vert.spv" "$file"
  
  if [ $? -ne 0 ]; then
      exit 1
  fi
  
  printf "Done!\n"
done

for file in $(find . -type f -name "*.frag"); do
  printf "Compiling \"%s\"... " "$file"
  
  filename=${file%.*}
  
  glslc -o "${filename}_frag.spv" "$file"
  
  if [ $? -ne 0 ]; then
      exit 1
  fi
  
  printf "Done!\n"
done