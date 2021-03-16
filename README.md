# Materialize_BatchMod
-----------------

I made this primarily for personal use, but I might aswell share it
all the output images are quickly made from default settings..
i thiink you can set the settings and then run it and they will all use those settings.. i dunno lol

very limited testing, as i made it very quickly for personal use, and I dont intend to support it, but it is open for anyone, it is a Mod of Materialize



### in the _Materialize-BatchMod folder is the built Unity program

on github here, is the Unity source files, with a _Materialize-BatchMod folder inside, 
_Materialize-BatchMod is the built unity project, thats all you really need, you could drag and place it anywhere on your PC

the images from the BatchMod output to:

...\_Materialize-BatchMod\Output\



Instructions for Use of BatchMod
--------------------------------
on the bottom left of the interface you will see 3 new buttons and 2 indicator lights, and a file number display

there is two modes

- Single - from a Single texture diffuse ->  create height, normal, smoothness, ambient occlusion 

- Folder - from a Whole Folder of diffuse textures -> create height, normal, smoothness, ambient occlusion from each one in the folder


> DO NOT start a new process until the current process you start is finished 
> 
> ( both green indicator lights are lit when BatchMod processes are idle )

## Single Mode

1. press OpenDiffuse, and select your diffuse texture image
2. press Process H N S Ao
3. the processes will run and give you a H, N, S, Ao images into ...\_Materialize-BatchMod\Output\ folder

- the process will go, and when both indicator lights are green, the process is done and ready for a new operation

## Folder Mode

1. press Folder H N S Ao
2. select the first file in the folder you want to process,  its advisable that the entire contents of the folder is entirely diffuse texture images (i personally only use .png so its only tested with .png)
3. the processes will run, outputting H, N, S, Ao images into ...\_Materialize-BatchMod\Output\ folder
4. the Files x / Max will increase, the left indicator flashes after each file is completed, and the right indicator will be green when the entire folder processes is complete


# Enjoy ! 
