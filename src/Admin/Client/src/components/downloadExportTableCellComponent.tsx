import React, { useCallback } from 'react';
import { Button, ButtonColor, ButtonSize } from '@kentico/xperience-admin-components';
import { usePageCommandProvider } from '@kentico/xperience-admin-base';

/** Copy of C# DownloadExportClientProperties class. */
interface DownloadExportClientProperties {
    fileName: string;
}

export const DownloadExportTableCellComponent = (props: DownloadExportClientProperties) => {
    const { executeCommand } = usePageCommandProvider();

    /**
     * Click handler for export download button.
     */
    const handleExportDownload = useCallback(async () => {
        const base64 = await executeCommand<string, string>("GetBase64String", props.fileName);
        const linkOptions = {
            style: { display: 'none' },
            href: `data:text/plain;base64,${base64}`,
            download: props.fileName
        };
        const link = Object.assign(document.createElement('a'), linkOptions);

        document.body.appendChild(link); console.log(link);
        link.click();

        link.parentNode?.removeChild(link);
    }, []);

    return (
        <Button
            icon='xp-arrow-down-line'
            color={ButtonColor.Quinary}
            size={ButtonSize.S}
            borderless={true}
            onClick={() => handleExportDownload()} />
    );
};
