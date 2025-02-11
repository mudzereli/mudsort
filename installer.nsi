!define APPNAME "mudsort"
!define SOFTWARECOMPANY "mudzereli"
!define APPGUID "{0a2f484b-54a6-4084-8286-c374231148a6}"
!define CLASSNAME "mudsort.PluginCore"
!define ASSEMBLY "mudsort.dll"
InstallDir "C:\Games\Decal Plugins\${APPNAME}"
;Icon "Installer\Res\Decal.ico"
!define BUILDPATH ".\bin\Release"

!getdllversion "${BUILDPATH}\${ASSEMBLY}" Expv_
!define VERSION ${Expv_1}.${Expv_2}.${Expv_3}

OutFile "${BUILDPATH}\${APPNAME}-installer-${VERSION}.exe"

; Main Install settings
; compressor goes first
SetCompressor LZMA

Name "${APPNAME} ${VERSION}"
InstallDirRegKey HKLM "Software\${SOFTWARECOMPANY}\${APPNAME}" ""
;SetFont "Verdana" 8

; Use compression

; Modern interface settings
!include "MUI.nsh"

!define MUI_ABORTWARNING

!insertmacro MUI_PAGE_WELCOME
;!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Set languages (first is default language)
!insertmacro MUI_LANGUAGE "English"
!insertmacro MUI_RESERVEFILE_LANGDLL

Section "" CoreSection
; Set Section properties
    SetOverwrite on

    ; Set Section Files and Shortcuts
    SetOutPath "$INSTDIR\"
    
    File "${BUILDPATH}\${ASSEMBLY}"
    ;File "${BUILDPATH}\example_sort_meta.met"

SectionEnd

Section -FinishSection

    WriteRegStr HKLM "Software\${SOFTWARECOMPANY}\${APPNAME}" "" "$INSTDIR"
    WriteRegStr HKLM "Software\${SOFTWARECOMPANY}\${APPNAME}" "Version" "${VERSION}"

    ;Register in decal
    ClearErrors
    ReadRegStr $0 HKLM "Software\Decal\Plugins\${APPGUID}" ""
    ${If} ${Errors}
        WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "" "${APPNAME}"
        WriteRegDWORD HKLM "Software\Decal\Plugins\${APPGUID}" "Enabled" "1"
        WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Object" "${CLASSNAME}"
        WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Assembly" "${ASSEMBLY}"
        WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Path" "$INSTDIR"
        WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Surrogate" "{71A69713-6593-47EC-0002-0000000DECA1}"
        WriteRegStr HKLM "Software\Decal\Plugins\${APPGUID}" "Uninstaller" "${APPNAME}"
    ${Else}
        ${IF} $0 != "${APPNAME}"
            MESSAGEBOX MB_OK|MB_ICONSTOP "Skipped decal plugin registration. A decal plugin with this GUID already exists ($0), and is not ${APPNAME}."
        ${ENDIF}
    ${EndIf}

    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$INSTDIR\uninstall.exe"
    WriteUninstaller "$INSTDIR\uninstall.exe"

SectionEnd

; Modern install component descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${CoreSection} ""
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;Uninstall section
Section Uninstall

    ;Remove from registry...
    DeleteRegKey HKLM "Software\${SOFTWARECOMPANY}\${APPNAME}"
    DeleteRegKey HKLM "Software\Decal\Plugins\${APPGUID}"
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"

    ; Delete self
    Delete "$INSTDIR\uninstall.exe"
    Delete "$INSTDIR\${ASSEMBLY}"
    RMDir "$INSTDIR\"

SectionEnd

; eof
