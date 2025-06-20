import React from 'react';
import {
    ActionMenu,
    ActionMenuHeadline,
    Button,
    ButtonColor,
    ButtonSize,
    Card,
    Cols,
    Column,
    MenuItem,
    Row,
    Spacing,
    Stack,
    TextArea
} from '@kentico/xperience-admin-components';
import { usePageCommand } from '@kentico/xperience-admin-base';

interface DatabaseTable {
    name: string;
    columns: string[];
}

interface EditQueryClientProperties {
    tables: DatabaseTable[];
    query: string | undefined;
}

export const EditQueryTemplate = (props: EditQueryClientProperties) => {
    const textAreaRef = React.createRef<HTMLTextAreaElement>();
    const { execute: runSql } = usePageCommand<void, string>("RunSql");

    const generateQuery = (table: DatabaseTable) => {
        const q = `SELECT ${table.columns.join(', ')} FROM ${table.name}`;
        if (textAreaRef.current) {
            textAreaRef.current.textContent = q;
        }
    };

    const runClick = () => {
        if (!textAreaRef.current) {
            return;
        }

        if (textAreaRef.current.textContent) {
            runSql(textAreaRef.current.textContent);
        }
        else {
            alert('Please enter a query to execute.');
        }
    };

    const copyQuery = async () => {
        if (textAreaRef.current) {
            await navigator.clipboard.writeText(textAreaRef.current.textContent ?? '');
        }
    };

    return (
        <>
            <Stack fullHeight={true} spacing={Spacing.XL}>
                <Button label="Run" color={ButtonColor.Primary} onClick={runClick} />

                <Row spacing={Spacing.XL}>
                    <Column cols={Cols.Col10}>
                        <Card headline='Query'>
                            <TextArea
                                minRows={10}
                                maxRows={40}
                                invalid={false}
                                disabled={false}
                                readOnly={false}
                                value={props.query ?? ''}
                                textAreaRef={textAreaRef}
                                placeholder='Enter SQL query...'
                                renderActions={() => (
                                    <>
                                        <Button label='Copy' size={ButtonSize.S} onClick={copyQuery} icon='xp-doc-copy' />
                                        <Button label='Save' size={ButtonSize.S} icon='xp-doc-plus' />
                                    </>
                                )} />
                        </Card>
                    </Column>

                    <Column cols={Cols.Col2}>
                        <ActionMenu>
                            <ActionMenuHeadline label='Tables' />
                            {
                                props.tables.map(table =>
                                    <MenuItem primaryLabel={table.name} onClick={() => generateQuery(table)} />
                                )
                            }
                        </ActionMenu>
                    </Column>
                </Row>
            </Stack>
        </>
    );
};
