// OpenConsoleCPPCLI.cpp : Définit les fonctions exportées de la DLL.
//

#include "pch.h"
#include "framework.h"
#include "OpenConsoleCPPCLI.h"


// Il s'agit d'un exemple de variable exportée
OPENCONSOLECPPCLI_API int nOpenConsoleCPPCLI=0;

// Il s'agit d'un exemple de fonction exportée.
OPENCONSOLECPPCLI_API int fnOpenConsoleCPPCLI(void)
{
    return 0;
}

// Il s'agit du constructeur d'une classe qui a été exportée.
COpenConsoleCPPCLI::COpenConsoleCPPCLI()
{
    return;
}
