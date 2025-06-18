import React from 'react';
import {
    Button,
    ButtonColor,
    ButtonSize,
    Card,
    Cols,
    Column,
    Row,
    Spacing,
    Stack,
    TextArea
} from '@kentico/xperience-admin-components';

export const EditQueryTemplate = () => {
    return (
        <Row spacing={Spacing.XL}>
            <Column
                cols={Cols.Col12}
                colsMd={Cols.Col10}
                colsLg={Cols.Col8}>

                <Stack spacing={Spacing.M}>
                    <Button label="Run" color={ButtonColor.Primary} />
                    <Card headline='Query'>
                        <TextArea
                            minRows={10}
                            maxRows={40}
                            invalid={false}
                            disabled={false}
                            readOnly={false}
                            placeholder='Enter SQL query...'
                            renderActions={() => (
                                <>
                                    <Button label="Copy" size={ButtonSize.S} icon="xp-doc-copy" />
                                    <Button label="Second Action" size={ButtonSize.S} icon="xp-doc-copy" />
                                </>
                            )}/>
                    </Card>
                </Stack>

            </Column>
        </Row>);
};
